using System;

namespace Suigetsu.Core.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="T:System.IComparable" /> interface.
    /// </summary>
    public static class ComparableExtensions
    {
        /// <summary>
        ///     Checks if the item is between the two provided items of the same type.
        /// </summary>
        public static bool IsBetween(this IComparable item, IComparable low, IComparable high)
        {
            return item.CompareTo(low) >= 0 && item.CompareTo(high) <= 0;
        }
    }
}
