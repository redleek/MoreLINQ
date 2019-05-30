using System;
using System.Collections.Generic;

namespace MoreLinq
{
    static partial class MoreEnumerable
    {
        /// <summary>
        /// Takes the source sequence and removes recurring items in series.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>The sequence with items occuring more than once in seris removed.</returns>
        /// <remarks>
        /// This operator uses deferred execution and streams its results (buckets and bucket content).
        /// </remarks>
        public static IEnumerable<TSource> Squash<TSource>(this IEnumerable<TSource> source) => source.Squash(_ => _);

        /// <summary>
        /// Takes the source sequence and removes recurring items in series using a key selector for comparison.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key elements selected from <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">The projection to apply to select a comparison key.</param>
        /// <returns>The sequence with items occuring more than once in seris removed.</returns>
        /// <remarks>
        /// This operator uses deferred execution and streams its results (buckets and bucket content).
        /// </remarks>
        public static IEnumerable<TSource> Squash<TSource, TKey>(this IEnumerable<TSource> source,
                                                                      Func<TSource, TKey> keySelector) => source.Squash(keySelector, null);

        /// <summary>
        /// Takes the source sequence and removes recurring items in series using a key selector for comparison using a given equality comparer.
        /// </summary>
        /// <typeparam name="TSource">ype of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key elements selected from <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">The projection to apply to select a comparison key.</param>
        /// <param name="equalityComparer">The comparer used to compare the equality of keys given by <paramref name="keySelector"/>.</param>
        /// <returns>The sequence with items occuring more than once in seris removed.</returns>
        /// <remarks>
        /// This operator uses deferred execution and streams its results (buckets and bucket content).
        /// </remarks>
        public static IEnumerable<TSource> Squash<TSource, TKey>(this IEnumerable<TSource> source,
                                                                      Func<TSource, TKey> keySelector,
                                                                      IEqualityComparer<TKey> equalityComparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            equalityComparer = equalityComparer ?? EqualityComparer<TKey>.Default;

            return _(); IEnumerable<TSource> _()
            {
                using (var enumer = source.GetEnumerator())
                {
                    enumer.MoveNext();
                    var prev = enumer.Current;

                    while (enumer.MoveNext())
                    {
                        if (!equalityComparer.Equals(keySelector(prev), keySelector(enumer.Current)))
                        {
                            yield return prev;
                            prev = enumer.Current;
                        }
                    }

                    yield return prev;
                }
            }
        }
    }
}
