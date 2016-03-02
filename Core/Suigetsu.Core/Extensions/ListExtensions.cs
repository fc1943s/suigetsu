using System.Collections.Generic;

namespace Suigetsu.Core.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="T:System.Collections.Generic.IList``1" /> interface.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        ///     Converts the list into a sequence starting from the last item.
        /// </summary>
        public static IEnumerable<T> FastReverse<T>(this IList<T> items)
        {
            for(var i = items.Count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }

        /// <summary>
        ///     Tests if the list is empty.
        /// </summary>
        public static bool IsEmpty<T>(this IList<T> list)
        {
            return list.Count <= 0;
        }
    }
}
 