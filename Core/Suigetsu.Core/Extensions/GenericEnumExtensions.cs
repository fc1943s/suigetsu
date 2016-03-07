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

        public static EnumRepo GetRepoById<T>(this T item, string id)
            where T : struct, IComparable, IFormattable, IConvertible => ById(item, id).GetRepo();

        public static EnumRepo GetRepo<T>(this T item) where T : struct, IComparable, IFormattable, IConvertible
            => EnumRepo.Get(item);

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public static T ById<T>(this T item, string id) where T : struct, IComparable, IFormattable, IConvertible
            => EnumRepo.EnumById<T>(id);
    }
}
