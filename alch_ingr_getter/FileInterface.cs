using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace alch_ingr_getter
{
    public class FileInterface
    {
        public FileInterface(string filepath)
        {
            File = new StreamWriter(filepath);
        }

        public StreamWriter File;

        public void WriteLine(string data)
        {
            Console.WriteLine();
            File.WriteLineAsync(data);
        }

        // Simply retrieves the name of a given effect
        private static string GetEffectName(IEffectGetter effect, IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            IFormLinkNullableGetter<IMagicEffectGetter> MGEF = effect.BaseEffect;
            var get = MGEF?.TryResolve(state.LinkCache);
            var name = get?.Name?.ToString();
            return name ?? "";
        }

        /** Outputs:
         *  magnitude = x.x
         *  duration = x
         */
        private static List<string> GetEffectData(IEffectGetter effect, string indent = "\t\t")
        {
            List<string> data = new();

            if (effect.Data == null)
                return data;

            data.Add(indent + "magnitude\t= " + effect.Data.Magnitude.ToString());
            data.Add(indent + "duration\t= " + effect.Data.Duration.ToString());

            return data;
        }

        /** Outputs (indentation-default = 2 tabs):
         *  Keywords
         *  {
         *      FORMID = EDITORID
         *      ...
         *  }
         */
        private static List<string> GetEffectKeywords(IEffectGetter effect, IPatcherState<ISkyrimMod, ISkyrimModGetter> state, string indent = "\t\t")
        {
            List<string> data = new();
            data.Add(indent + "Keywords");
            data.Add(indent + '{');
            foreach(var link in effect.ContainedFormLinks.Where(_ => !_.IsNull))
            {
                link.TryResolveCommon(state.LinkCache, out var resolvedLink);
                if (resolvedLink == null || resolvedLink.EditorID == null)
                    continue;
                data.Add(indent + '\t' + resolvedLink.FormKey.IDString() + " = " + resolvedLink.EditorID);
            }
            data.Add(indent + '}');
            return data;
        }
        // Write a list of strings to file, each element is one line.
        private void Write(ref List<string> data)
        {
            foreach(var line in data)
            {
                File.WriteLine(line);
                Console.WriteLine(line);
            }
        }

        public int WriteIngredient(IIngredientGetter ingr, IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            string? name = ingr.Name?.ToString();
            if (name == null)
                return 0;

            List<string> data = new(); // discarded after use

            data.Add(name);
            data.Add("{");
            foreach(var effect in ingr.Effects)
            {
                data.Add("\t" + GetEffectName(effect, state));
                data.Add("\t{");
                data.AddRange(GetEffectData(effect));
                data.AddRange(GetEffectKeywords(effect, state));
                data.Add("\t}");
            }
            data.Add("}");

            if (data.Count == 0)
                return 0;
            Write(ref data);
            return 1;
        }
    }
}
