using System;
using System.Collections.Generic;
using Suigetsu.Core.Extensions;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Enums
{
    /// <summary>
    ///     <see cref="Attribute" /> that provides a way to store data on <see langword="enum" /> items and retrieve them
    ///     easily.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class BaseEnumRepo : Attribute
    {
        /// <summary>
        ///     The <see langword="enum" /> item that contains this current attribute's instance.
        /// </summary>
        public Enum EnumItem;

        /// <summary>
        ///     Creates a <paramref name="data" /> repository for the current <see langword="enum" /> item.
        /// </summary>
        protected BaseEnumRepo(params object[] data)
        {
            Data = data;
        }

        /// <summary>
        ///     Storage for the attribute's args.
        /// </summary>
        protected object[] Data { get; }

        /// <summary>
        ///     Getter for the stored <see cref="Data" /> objects.
        /// </summary>
        public object this[int index]
        {
            get
            {
                if(index < 0 || index >= Data.Length)
                {
                    return null;
                }

                return Data[index];
            }
        }

        public static IEnumerable<TRepo> GetAttributes<TRepo>(Type type) where TRepo : BaseEnumRepo
        {
            foreach(var v in Enum.GetValues(type))
            {
                foreach(var v2 in type.GetMember(v.ToString())[0].GetCustomAttributes(typeof(TRepo), false))
                {
                    if(((TRepo)v2).EnumItem == null)
                    {
                        ((TRepo)v2).EnumItem = (Enum)v;
                    }
                    yield return (TRepo)v2;
                }
            }
        }

        private static IEnumerable<TRepo> GetAttributes<TRepo, TEnum>() where TRepo : BaseEnumRepo
            where TEnum : struct, IComparable, IFormattable, IConvertible => GetAttributes<TRepo>(typeof(TEnum));

        private static TRepo GetEmptyRepo<TRepo>(Type type) where TRepo : BaseEnumRepo, new() => new TRepo
        {
            EnumItem = EnumUtils.GetEmpty(type)
        };

        private static TRepo GetEmptyRepo<TEnum, TRepo>() where TRepo : BaseEnumRepo, new() where TEnum : struct, IFormattable
            => GetEmptyRepo<TRepo>(typeof(TEnum));

        /// <summary>
        ///     Searches for an <see cref="EnumRepo" /> using the given <see langword="enum" /> type.
        /// </summary>
        protected static TRepo FindEnum<TRepo>(Type type, Func<TRepo, bool> validation) where TRepo : BaseEnumRepo, new()
            => GetAttributes<TRepo>(type).FirstOrDefault(validation, () => GetEmptyRepo<TRepo>(type));

        private static TRepo FindEnum<TEnum, TRepo>(Func<TRepo, bool> validation) where TRepo : BaseEnumRepo, new()
            where TEnum : struct, IComparable, IFormattable, IConvertible
            => GetAttributes<TRepo, TEnum>().FirstOrDefault<TRepo>(validation, GetEmptyRepo<TEnum, TRepo>);

        /// <summary>
        ///     Searches for an <see cref="EnumRepo" /> using the given <see langword="enum" />
        ///     <paramref name="item" />'s type.
        /// </summary>
        protected static TRepo GetRepo<TRepo>(Enum item) where TRepo : BaseEnumRepo, new()
            => FindEnum<TRepo>(item?.GetType() ?? typeof(EnumUtils.EmptyEnum), x => Equals(x.EnumItem, item));

        /// <summary>
        ///     Searches for an <see cref="EnumRepo" /> using the given <see langword="enum" />
        ///     <paramref name="item" />'s type.
        /// </summary>
        protected static TRepo GetRepo<TEnum, TRepo>(TEnum item) where TRepo : BaseEnumRepo, new()
            where TEnum : struct, IComparable, IFormattable, IConvertible
            => FindEnum<TEnum, TRepo>(x => Equals(x.EnumItem, item));
    }
}
