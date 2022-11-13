using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mutagen.alch_registry_builder
{
    // these match the definitions in the alch project
    public enum EKeywordDisposition : byte
    {
        Unknown = 0,
        Neutral = 1,
        Positive = 2,
        Cure = 4,
        FortifyStat = 8,
        Negative = 16,
        InfluenceOther = 32,
    };

    public struct Keyword
    {
        public string name;
        public string formID;
        public EKeywordDisposition disposition;

        public static Keyword FromGetter(IKeywordGetter keywordGetter) => new()
        {
            name = keywordGetter.EditorID ?? string.Empty,
            formID = keywordGetter.FormKey.IDString(),
            disposition = GetDisposition(keywordGetter)
        };

        #region GetDisposition
        private class KeywordDispositionMatcher : IEnumerable<string>, IList<string>
        {
            public List<string> expressions = new();

            public bool IsMatch(string name) => expressions?.Any(expr => Regex.IsMatch(name, expr, RegexOptions.Singleline)) ?? false;

            #region Interfaces
            public string this[int index] { get => ((IList<string>)expressions)[index]; set => ((IList<string>)expressions)[index] = value; }

            public int Count => ((ICollection<string>)expressions).Count;

            public bool IsReadOnly => ((ICollection<string>)expressions).IsReadOnly;

            public void Add(string item) => ((ICollection<string>)expressions).Add(item);
            public void Clear() => ((ICollection<string>)expressions).Clear();
            public bool Contains(string item) => ((ICollection<string>)expressions).Contains(item);
            public void CopyTo(string[] array, int arrayIndex) => ((ICollection<string>)expressions).CopyTo(array, arrayIndex);
            public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)expressions).GetEnumerator();
            public int IndexOf(string item) => ((IList<string>)expressions).IndexOf(item);
            public void Insert(int index, string item) => ((IList<string>)expressions).Insert(index, item);
            public bool Remove(string item) => ((ICollection<string>)expressions).Remove(item);
            public void RemoveAt(int index) => ((IList<string>)expressions).RemoveAt(index);
            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)expressions).GetEnumerator();
            #endregion Interfaces
        }

        private static readonly Dictionary<EKeywordDisposition, KeywordDispositionMatcher> DispositionMap = new()
        {
            { EKeywordDisposition.Unknown, new() },
            { EKeywordDisposition.Neutral, new() },
            {
                EKeywordDisposition.Positive, new() {
                    "Beneficial",
                    "Restore",
                    "FortifyHealth",
                    "FortifyStamina",
                    "FortifyMagicka",
                    "Strength",
                    "Resist",
            }},
            {
                EKeywordDisposition.Cure, new()
                {
                    "Cure"
                }
            },
            {
                EKeywordDisposition.FortifyStat, new()
                {
                    "Fortify",
                }
            },
            {
                EKeywordDisposition.Negative,
                new()
                {
                    "Harmful",
                    "Damage",
                    "Fatigue",
                    "Silence",
                    "Weakness",
                }
            },
            {
                EKeywordDisposition.InfluenceOther,
                new()
                {
                    "Influence"
                }
            }
        };
        private static EKeywordDisposition GetDisposition(IKeywordGetter keyword)
        {
            if (keyword.EditorID is null) return EKeywordDisposition.Unknown;
            foreach (var (disposition, matcher) in DispositionMap)
            {
                if (matcher.IsMatch(keyword.EditorID))
                {
                    return disposition;
                }
            }
            return EKeywordDisposition.Neutral;
        }
        #endregion GetDisposition
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
            magicEffectGetter.Keywords?.ForEach(link =>
            {
                if (link.TryResolve(linkCache) is IKeywordGetter keywordGetter)
                    kwda.Add(Keyword.FromGetter(keywordGetter));
            });
            return new()
            {
                name = magicEffectGetter.Name?.String ?? string.Empty,
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
                name = ingredientGetter.Name?.String ?? string.Empty,
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
        public static GameSetting FromGetter(IGameSettingStringGetter gameSettingGetter) => new() { name = gameSettingGetter.EditorID ?? string.Empty, value = gameSettingGetter.Data?.String ?? string.Empty };
    }
    public struct Registry
    {
        public Registry() { }
        //public readonly List<GameSetting> GameSettings = new();
        public readonly List<Ingredient> Ingredients = new();
    }
}
