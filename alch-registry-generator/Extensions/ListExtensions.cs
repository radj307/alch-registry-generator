using System.Collections.Generic;

namespace Mutagen.alch_registry_builder.Extensions
{
    public static class ListExtensions
    {
        public static void AddIfUnique<T>(this IList<T> list, T item)
        {
            if (list.Contains(item)) return;
            list.Add(item);
        }
    }
}
