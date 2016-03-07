using System;
using System.Collections.Generic;
using System.Linq;

namespace Suigetsu.Core.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="T:System.Collections.Generic.IEnumerable``1" /> interface.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     Returns the first element of the sequence that satisfies a condition or a default value if no such element is
        ///     found.
        /// </summary>
        public static T FirstOrDefault<T>(this IEnumerable<T> list, Func<T, bool> predicate, Func<T> defaultValue)
        {
            var value = predicate != null ? list.FirstOrDefault(predicate) : list.FirstOrDefault();
            if(Equals(value, default(T)))
            {
                value = defaultValue();
            }
            return value;
        }

        /// <summary>
        ///     Returns the first element of the sequence that satisfies a condition or a default value if no such element is
        ///     found.
        /// </summary>
        public static T FirstOrDefault<T>(this IEnumerable<T> list, Func<T> defaultValue)
            => FirstOrDefault(list, null, defaultValue);

        /// <summary>
        ///     Wraps each item with the given <paramref name="text" />, converting them into a string.
        /// </summary>
        public static IEnumerable<string> Wrap<T>(this IEnumerable<T> list, string text)
            => list.Select(x => x.ToString().Wrap(text));

        /// <summary>
        ///     Wraps each item with the given <paramref name="before" /> and
        ///     <paramref name="after" /> parameters, converting them into a string.
        /// </summary>
        public static IEnumerable<string> Wrap<T>(this IEnumerable<T> list, string before, string after)
            => list.Select(x => x.ToString().Wrap(before, after));

        /// <summary>
        ///     Sequence splitter that supports more than one value as delimiter.
        /// </summary>
        /// <param name="seq">Seq to be split.</param>
        /// <param name="delimiters">Items that will be used as delimiter to split the sequence.</param>
        /// <param name="onSplitDelimiterFound">Invoked with the index of the first delimiter for each item found.</param>
        public static IList<T[]> Split<T>(this IEnumerable<T> seq, IEnumerable<T> delimiters,
                                          Action<int> onSplitDelimiterFound = null)
        {
            var result = new List<T[]>();

            var start = 0;

            var seqArray = seq as T[] ?? seq.ToArray();
            var delimitersArray = delimiters as T[] ?? delimiters.ToArray();

            if(delimitersArray.IsEmpty())
            {
                result.Add(seqArray);
                return result;
            }

            for(var i = 0; i < seqArray.Length; i++)
            {
                var iClosure = i;

                var delimiterFound =
                    !delimitersArray.Where
                         ((x, j) => !seqArray[iClosure + j].Equals(x) // Different item
                                    || (iClosure + j) >= seqArray.Length // Out of the seq boundaries
                         ).Any();

                if(delimiterFound)
                {
                    onSplitDelimiterFound?.Invoke(i);
                }
                else if(i != seqArray.Length - 1)
                {
                    continue; // Loops until we find a delimiter
                }

                // At this point we found a delimiter or we are at the end of the sequence.
                // Creates an array of a size that can contain the current item.
                // (If we are at the end of the seq, and it's not a delimiter, add it and finish)
                var currentList = new T[((!delimiterFound ? 1 : 0) + i) - start];

                for(var j = 0; j < currentList.Length; j++)
                {
                    currentList[j] = seqArray[j + start];
                }

                if(!currentList.IsEmpty())
                {
                    result.Add(currentList);
                }

                // Position our loop at the start of the next item
                if(delimitersArray.Length > 1)
                {
                    i += delimitersArray.Length - 1;
                }

                start = i + 1;
            }
            return result;
        }
    }
}
