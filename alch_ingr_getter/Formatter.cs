using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace alch_ingr_getter
{
    public static class Formatter
    {
        // Simply retrieves the name of a given effect
        private static string GetEffectName(IEffectGetter effect, IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            IFormLinkNullableGetter<IMagicEffectGetter> MGEF = effect.BaseEffect;
            var get = MGEF?.TryResolve(state.LinkCache);
            var name = get?.Name?.ToString();
            return name ?? "";
        }

        private static List<string> GetEffectData(IEffectGetter effect, string indent = "\t\t")
        {
            List<string> data = new();

            if (effect.Data == null)
                return data;

            data.Add(indent + "magnitude\t= " + effect.Data.Magnitude.ToString());
            data.Add(indent + "duration\t= " + effect.Data.Duration.ToString());

            return data;
        }

        private static List<string> GetEffectKeywords(IEffectGetter effect, IPatcherState<ISkyrimMod, ISkyrimModGetter> state, string indent = "\t\t")
        {
            List<string> data = new();
            data.Add(indent + "Keywords");
            data.Add(indent + '{');
            foreach (var link in effect.ContainedFormLinks.Where(_ => !_.IsNull))
            {
                link.TryResolveCommon(state.LinkCache, out var resolvedLink);
                if (resolvedLink == null || resolvedLink.EditorID == null)
                    continue;
                data.Add(indent + '\t' + resolvedLink.FormKey.IDString() + " = " + resolvedLink.EditorID);
            }
            data.Add(indent + '}');
            return data;
        }

        public static List<string> FormatIngredient(IIngredientGetter ingr, IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            string? name = ingr.Name?.ToString();

            List<string> data = new(); // discarded after use

            if (name == null)
                return data;

            data.Add(name);
            data.Add("{");
            foreach (var effect in ingr.Effects)
            {
                data.Add("\t" + GetEffectName(effect, state));
                data.Add("\t{");
                data.AddRange(GetEffectData(effect));
                data.AddRange(GetEffectKeywords(effect, state));
                data.Add("\t}");
            }
            data.Add("}");
            return data;
        }
    }
}
