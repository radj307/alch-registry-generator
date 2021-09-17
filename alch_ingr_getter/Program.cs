using System;
using System.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace alch_ingr_getter
{
    public static class Program
    {
        internal static readonly string filepath = "alch.ingredients";
        internal static FileInterface IO = new(filepath);

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "YourPatcher.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            Console.WriteLine("\n\n=== Beginning Process. ===\nGetting a list of all ingredients...\n");

            int count = 0;
            foreach( var ingr in state.LoadOrder.PriorityOrder.Ingredient().WinningOverrides().Where(i => i.EditorID != null))
                count += IO.WriteIngredient(ingr, state);

            Console.WriteLine($"\n\n=== Process Completed. ===\nSuccessfully wrote {count} ingredients to \"{filepath}\"\n");
        }
    }
}
