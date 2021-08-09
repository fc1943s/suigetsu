using System;
using Suigetsu.Core.Extensions;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Enums
{
    /// <summary>
    ///     <see cref="Attribute" /> that provides a way to store data on <see langword="enum" /> items and retrieve them
    ///     easily.
    /// </summary>
    public class EnumRepo : BaseEnumRepo
    {
        //TODO: ensures only one ID
        private readonly string _id;

        /// <summary>
        ///     Creates a <paramref name="data" /> repository for the current <see langword="enum" /> item.
        /// </summary>
        public EnumRepo(string id, string desc, params object[] data) : base(data)
        {
            _id = id ?? string.Empty;
            Desc = desc ?? string.Empty;
        }

        /// <summary>
        ///     Creates a <paramref name="data" /> repository for the current <see langword="enum" /> item.
        /// </summary>
        public EnumRepo(string id, params object[] data) : this(id, string.Empty, data) {}

        /// <summary>
        ///     Creates a data repository for the current <see langword="enum" /> item.
        /// </summary>
        public EnumRepo() : this(string.Empty) {}

        /// <summary>
        ///     Id representation of the <see langword="enum" /> item. Can be used for database storage.
        /// </summary>
        public string Id
            => _id.IsEmpty() && !Equals(EnumUtils.GetEmpty(EnumItem.GetType()), EnumItem) ? EnumItem.ToString() : _id;

        /// <summary>
        ///     Description of the <see langword="enum" /> item. Can be used for drop down population.
        /// </summary>
        public string Desc { get; }

        /// <summary>
        ///     Retrieves the arg at the given <paramref name="index" />, returning a default value if not found.
        /// </summary>
        public T Arg<T>(int index)
        {
            if(index < 0 || index >= Data.Length)
            {
                return TypeUtils.Default<T>();
            }

            return (T)Data[index];
        }

        /// <summary>
        ///     If the arg at the given <paramref name="index" /> is an <see langword="enum" /> item, retrieves
        ///     its own <see cref="EnumRepo" />.
        /// </summary>
        public EnumRepo ArgAsRepo(int index) => Get((Enum)Data[index]);

        /// <summary>
        ///     Retrieves the repo from the given <see langword="enum" /> item.
        /// </summary>
        public static EnumRepo Get<T>(T item) where T : struct, IComparable, IFormattable, IConvertible
        => GetRepo<T, EnumRepo>(item);

        /// <summary>
        ///     Retrieves the repo from the given <see langword="enum" /> item.
        /// </summary>
        public static EnumRepo Get(Enum item) => GetRepo<EnumRepo>(item);

        /// <summary>
        ///     Searches an <see langword="enum" /> item using the repo's <paramref name="id" />.
        /// </summary>
        public static Enum EnumById(Type type, string id) => FindEnum<EnumRepo>(type, x => x.Id == id).EnumItem;

        /// <summary>
        ///     Searches the first <see langword="enum" /> item using the repo's <paramref name="desc" />.
        /// </summary>
        private static Enum EnumByDesc(Type type, string desc) => FindEnum<EnumRepo>(type, x => x.Desc == desc).EnumItem;

        /// <summary>
        ///     Searches an <see langword="enum" /> item using the repo's <paramref name="id" />.
        /// </summary>
        public static T EnumById<T>(string id) where T : struct, IComparable, IFormattable, IConvertible
        => (T)(object)EnumById(typeof(T), id);

        /// <summary>
        ///     Searches the first <see langword="enum" /> item using the repo's <paramref name="desc" />.
        /// </summary>
        public static T EnumByDesc<T>(string desc) where T : struct, IComparable, IFormattable, IConvertible
        => (T)(object)EnumByDesc(typeof(T), desc);
    }
}
