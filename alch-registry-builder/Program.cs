using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Skyrim;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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
            // Initialize GameEnvironment
            var env = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE);

            // Set the output directory
            string out_path = GetOutPath("alch.ingredients");

            // Initialize FileInterface
            FileInterface IO = new(out_path);
            // Initialize output data list
            List<string> data = new();

            Console.WriteLine($"\nConfiguration\n");
            Console.WriteLine($"\tOutput:\t{out_path}");

            Console.WriteLine("\n\n=== Beginning Process. ===\nGetting a list of all ingredients...\n");

            int count = 0;
            foreach (var ingr in env.LoadOrder.PriorityOrder.Ingredient().WinningOverrides().Where(i => i.EditorID != null))
            {
                data.AddRange(Formatter.FormatIngredient(ingr, env));
                ++count;
            }

            Console.WriteLine("\n\n=== Process Completed. ===\nWriting to file...\n");

            IO.Write(ref data);

            Console.WriteLine($"Successfully wrote {count} ingredient{(count > 1 ? "s" : "")} to \"{out_path}\"\n");
        }
    }
}
