using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.Collections.Generic;

namespace Mutagen.alch_registry_builder
{
    // these match the definitions in the alch project
    public enum EKeywordDisposition : byte
    {
        Unknown = 0,
        Neutral = 1,
        Positive = 2,
        Cure = 4 | Positive,
        FortifyStat = 8 | Positive,
        Negative = 16,
        InfluenceOther = 32,
    };

    public struct Keyword
    {
        public string name;
        public string formID;

        public static Keyword FromGetter(IKeywordGetter keywordGetter) => new()
        {
            name = keywordGetter.EditorID ?? string.Empty,
            formID = keywordGetter.FormKey.IDString()
        };
    }
    public struct Effect
    {
        public string name;
        public float magnitude;
        public int duration;
        public List<Keyword> keywords;

        public static Effect FromGetter(IEffectGetter effectGetter, ILinkCache linkCache)
        {
            var magicEffectGetter = effectGetter.BaseEffect.TryResolve(linkCache);
            if (magicEffectGetter is null) throw new System.Exception($"Cannot build effect data structure; Failed to resolve formkey: {effectGetter.BaseEffect.FormKey}");
            List<Keyword> kwda = new();
            magicEffectGetter.Keywords?.ForEach(link => kwda.Add(Keyword.FromGetter(link.TryResolve(linkCache))));
            return new()
            {
                name = magicEffectGetter.EditorID ?? string.Empty,
                magnitude = effectGetter.Data?.Magnitude ?? -0f,
                duration = effectGetter.Data?.Duration ?? 0,
                keywords = kwda,
            };
        }
    }
    public struct Ingredient
    {
        public string name;
        public List<Effect> effects;

        public static Ingredient FromGetter(IIngredientGetter ingredientGetter, ILinkCache linkCache)
        {
            List<Effect> effects = new();
            ingredientGetter.Effects.ForEach(fx => effects.Add(Effect.FromGetter(fx, linkCache)));
            return new()
            {
                name = ingredientGetter.EditorID ?? string.Empty,
                effects = effects
            };
        }
    }
    public struct GameSetting
    {
        public string name;
        public object value;

        public static GameSetting FromGetter(IGameSettingBoolGetter gameSettingGetter) => new() { name = gameSettingGetter.EditorID ?? string.Empty, value = gameSettingGetter.Data ?? default };
        public static GameSetting FromGetter(IGameSettingFloatGetter gameSettingGetter) => new() { name = gameSettingGetter.EditorID ?? string.Empty, value = gameSettingGetter.Data ?? default };
        public static GameSetting FromGetter(IGameSettingIntGetter gameSettingGetter) => new() { name = gameSettingGetter.EditorID ?? string.Empty, value = gameSettingGetter.Data ?? default };
        public static GameSetting FromGetter(IGameSettingStringGetter gameSettingGetter) => new() { name = gameSettingGetter.EditorID ?? string.Empty, value = gameSettingGetter.Data ?? default };
    }
    public struct Registry
    {
        public Registry() { }
        public readonly List<Ingredient> ingredients = new();
        public readonly List<GameSetting> gameSettings = new();
    }
}
