using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Skyrim;
using System;
using System.IO;
using System.Linq;

namespace Mutagen.alch_registry_builder
{
    internal static class Program
    {
        // Accepts "-n"|"--no-pause" & "-o <name>"|"--output <name>"
        private static (string, bool) ParseArguments(string default_out_name, string[] args)
        {
            bool pauseBeforeExit = true;
            string? name = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("-n", StringComparison.Ordinal) || args[i].Equals("--no-pause", StringComparison.Ordinal))
                    pauseBeforeExit = false;
                else if (args[i].Equals("-o", StringComparison.Ordinal) || args[i].Equals("--output", StringComparison.Ordinal))
                {
                    if (i + 1 < args.Length)
                        name = args[++i];
                    else
                        throw new Exception("Output specifier included without a filename!");
                }
            }
            if (name != null)
            {
                if (!Path.IsPathFullyQualified(name)) // not absolute
                {
                    if (Path.IsPathRooted(name)) // relative
                        name = Path.GetFullPath(name);
                    else // name only contains a filename, resolve it from the current working directory
                        name = Path.GetFullPath(name, Directory.GetCurrentDirectory());
                } // else name is fully qualified, don't modify it
            }
            else name = Path.GetFullPath(default_out_name, Directory.GetCurrentDirectory());
            return (name, pauseBeforeExit);
        }

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
                var (out_path, do_pause) = ParseArguments("alch.ingredients", args);
                try
                {
                    // Initialize GameEnvironment
                    var env = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE);

                    var color_path = ConsoleColor.Yellow;
                    var color_header = ConsoleColor.Cyan;

                    // Initialize FileInterface
                    FileInterface O = new(out_path);

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
                        O.Write(Formatter.FormatIngredient(ingr, env));
                        LogLine("  DONE");
                    }

                    LogLine("=== Complete ===", color_header);
                    LogLine();

                    O.Close();

                    Log("Output written to \"");
                    Log(out_path, color_path);
                    LogLine("\"");

                    if (do_pause)
                    {
                        Console.Write("Press any key to exit...");
                        Console.ReadKey();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ERROR]\t{ex}\nPress any key to exit...\n");
                    Console.ResetColor();
                    if (do_pause)
                    {
                        Console.Write("Press any key to exit...");
                        Console.ReadKey();
                    }
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
