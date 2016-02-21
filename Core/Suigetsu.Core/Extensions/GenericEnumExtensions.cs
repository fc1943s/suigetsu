using System;
using System.Diagnostics.CodeAnalysis;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Extensions
{
    /// <summary>
    ///     Extensions for generic enum types.
    /// </summary>
    public static class GenericEnumExtensions
    {
        /// <summary>
        ///     <para>
        ///         Returns an instance of the provided enum type, with the stored value of
        ///         <see cref="F:Suigetsu.Core.Util.EnumUtils.InvalidEnumItem" />.
        ///     </para>
        ///     Shortcut for: <seealso cref="M:Suigetsu.Core.Util.EnumUtils.GetEmpty(System.Type)" />
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public static T GetEmpty<T>(this T item) where T : struct, IComparable, IFormattable, IConvertible
        {
            return (T)(object)EnumUtils.GetEmpty(typeof(T));
        }

        /// <summary>
        ///     Tests if the given enum item is empty/invalid, according to the
        ///     <see cref="F:Suigetsu.Core.Util.EnumUtils.InvalidEnumItem" /> value.
        /// </summary>
        public static bool IsEmptyEnum<T>(this T item) where T : struct, IComparable, IFormattable, IConvertible
        {
            return Equals(item, EnumUtils.GetEmpty(typeof(T)));
        }
    }
}
