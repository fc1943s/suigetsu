using System;
using System.Diagnostics.CodeAnalysis;
using Suigetsu.Core.Enums;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Extensions
{
    /// <summary>
    ///     Extensions for generic <see langword="enum" /> types.
    /// </summary>
    public static class GenericEnumExtensions
    {
        /// <summary>
        ///     <para>
        ///         Returns an instance of the provided <see langword="enum" /> type, with the stored value of
        ///         <see cref="F:Suigetsu.Core.Util.EnumUtils.InvalidEnumItem" />.
        ///     </para>
        ///     Shortcut for: <seealso cref="M:Suigetsu.Core.Util.EnumUtils.GetEmpty(System.Type)" />
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public static T GetEmpty<T>(this T item) where T : struct, IComparable, IFormattable, IConvertible
            => (T)(object)EnumUtils.GetEmpty(typeof(T));

        /// <summary>
        ///     Tests if the given <see langword="enum" /> item is empty/invalid, according to the
        ///     <see cref="F:Suigetsu.Core.Util.EnumUtils.InvalidEnumItem" /> value.
        /// </summary>
        public static bool IsEmptyEnum<T>(this T item) where T : struct, IComparable, IFormattable, IConvertible
            => Equals(item, EnumUtils.GetEmpty(typeof(T)));

        /// <summary>
        ///     Searches an <see cref="EnumRepo" /> using the given <paramref name="id" />.
        /// </summary>
        public static EnumRepo GetRepoById<T>(this T item, string id)
            where T : struct, IComparable, IFormattable, IConvertible => ById(item, id).GetRepo();

        /// <summary>
        ///     Retrieves the <see cref="EnumRepo" /> from the given <see langword="enum" /> <paramref name="item" />.
        /// </summary>
        public static EnumRepo GetRepo<T>(this T item) where T : struct, IComparable, IFormattable, IConvertible
            => EnumRepo.Get(item);

        /// <summary>
        ///     Searches an <see langword="enum" /> <paramref name="item" /> of the current type using
        ///     the given <paramref name="id" />.
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public static T ById<T>(this T item, string id) where T : struct, IComparable, IFormattable, IConvertible
            => EnumRepo.EnumById<T>(id);

        /// <summary>
        ///     Searches the first <see langword="enum" /> <paramref name="item" /> of the current type using
        ///     the given <paramref name="desc" />.
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public static T ByDesc<T>(this T item, string desc) where T : struct, IComparable, IFormattable, IConvertible
            => EnumRepo.EnumByDesc<T>(desc);
    }
}
