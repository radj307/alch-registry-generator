using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Skyrim;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Mutagen.alch_registry_builder
{
    internal static class Program
    {
        internal static string GetOutPath(string filename)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), filename);
        }

        static void Main(string[] args)
        {
            try
            {
                // Initialize GameEnvironment
                var env = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE);

                // Set the output directory
                string out_path = GetOutPath("alch.ingredients");

                // Initialize FileInterface
                FileInterface O = new(out_path);

                Console.WriteLine($"\nConfiguration\n");
                Console.WriteLine($"\tOutput:\t{out_path}");

                Console.WriteLine("\n\n=== Beginning Process. ===\n\nGetting a list of all ingredients...\n");

                int count = 0;
                foreach (var ingr in env.LoadOrder.PriorityOrder.Ingredient().WinningOverrides().Where(i => i.EditorID != null))
                {
                    Console.WriteLine($"[{++count}]\tFound ingredient \"{ingr.Name}\"");
                    O.Write(Formatter.FormatIngredient(ingr, env));
                }

                Console.WriteLine("\n\n=== Process Completed. ===\n");

                O.Close();

                Console.WriteLine($"Successfully wrote {count} ingredient{(count > 1 ? "s" : "")} to file:\n\"{out_path}\"\n");
                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }
    }
}
