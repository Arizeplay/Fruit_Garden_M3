using System;
using System.Collections.Generic;

namespace florius.Common
{
    public static class DistinctByMoreLINQ
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            foreach (var element in source)
                if (knownKeys.Add(keySelector(element)))
                    yield return element;
        }
    }
}