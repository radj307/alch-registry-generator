using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Skyrim;
using Newtonsoft.Json;
using Noggog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Mutagen.alch_registry_builder
{
    internal static class Program
    {
        static void Log(string message, ConsoleColor? color = null)
        {
            Console.ForegroundColor = color ?? Console.ForegroundColor;
            Console.Write(message);
            if (color != null)
                Console.ResetColor();
        }
        static void LogLine(string? message = null, ConsoleColor? color = null)
        {
            if (message != null)
                Log(message + '\n', color);
            else Log("\n", color);
        }

        static void Main(string[] args)
        {
            try
            {
                if (args.Contains("-h") || args.Contains("--help"))
                {
                    Console.Write("alch-registry-generator\n" +
                        "  Creates an ingredients registry for the `alch` program by parsing your load order.\n" +
                        "  Run this executable through your preferred mod manager for the best results.\n" +
                        "\n" +
                        "OPTIONS:\n" +
                        "  -h, --help            Shows this help display.\n" +
                        "  -n, --no-pause        Prevents the program from waiting for input before exiting.\n" +
                        "  -o, --output <PATH>   Specifies a path to write the output file to.\n" +
                        "  -H                    Indents the JSON file output so it's easier to read.\n" +
                        "  -O                    Opens the JSON file in the default external handler application.\n" +
                        "      --use-enum-names  Serializes enums as strings instead of integrals.");
                    return;
                }

                string out_path = "alch.ingredients";

                // figure out the output path
                for (int i = 0; i < args.Length; ++i)
                {
                    string arg = args[i];
                    if (arg.Equals("-o") || arg.Equals("--output"))
                    {
                        if (i == args.Length - 1) throw new Exception($"Argument \"{arg}\" must be followed by a valid output path.");
                        out_path = args[i + 1];
                    }
                }

                if (!Path.IsPathFullyQualified(out_path))
                { // get absolute path:
                    out_path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, out_path));
                }
                if (!Path.HasExtension(out_path))
                {
                    out_path = Path.Combine(out_path, "alch.ingredients");
                }

                bool do_pause = !args.Contains("-n") && !args.Contains("--no-pause");
                bool do_open = args.Contains("-O");

                JsonSerializerSettings serializerSettings = new()
                {
                    Formatting = args.Contains("-H") ? Formatting.Indented : Formatting.None
                };
                if (args.Contains("--use-enum-names"))
                    serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

                Registry registry = new();

                // Initialize GameEnvironment
                var env = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE);

                var color_path = ConsoleColor.Yellow;
                var color_header = ConsoleColor.Cyan;

                Log("Configuration: \n");
                Log("\tOutput:\t\"");
                Log(out_path, color_path);
                LogLine("\"");
                LogLine($"\tPause:\t{do_pause}");

                LogLine();
                LogLine("=== Begin ===", color_header);

                int count = 0;
                foreach (var ingr in env.LoadOrder.PriorityOrder.Ingredient().WinningOverrides().Where(i => i.EditorID != null))
                {
                    Log($"[{++count}]\tResolving FormLinks for \"{ingr.Name}\"...");
                    registry.Ingredients.Add(Ingredient.FromGetter(ingr, env.LinkCache));
                    LogLine("  DONE");
                }

                LogLine("=== Complete ===", color_header);
                LogLine();

                // Write output:
                using (StreamWriter sw = new(File.Open(out_path, FileMode.Create, FileAccess.Write, FileShare.None)))
                {
                    sw.Write(JsonConvert.SerializeObject(registry, serializerSettings));
                    sw.Flush();

                    Log("Output written to \"");
                    Log(out_path, color_path);
                    LogLine("\"");
                }

                if (do_open)
                {
                    Process.Start(new ProcessStartInfo(out_path)
                    {
                        Verb = "open",
                        UseShellExecute = true
                    });
                    Console.WriteLine("Opened output file with default handler application.");
                }

                if (do_pause)
                {
                    Console.Write("Press any key to exit...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR]\t{ex} occurred while parsing arguments.");
                Console.ResetColor();
            }
        }
    }
}
