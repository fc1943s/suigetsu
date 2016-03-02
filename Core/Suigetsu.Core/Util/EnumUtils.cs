using System;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Util
{
    /// <summary>
    ///     Utility methods for the <see cref="T:System.Enum" /> class.
    /// </summary>
    public static class EnumUtils
    {
        /// <summary>
        ///     Dummy <see langword="enum" /> for validation and testing purposes.
        /// </summary>
        public enum DummyEnum
        {
            /// <summary>
            ///     Dummy enum item for validation and testing purposes.
            /// </summary>
            DummyEnumItem
        }

        /// <summary>
        ///     Empty <see langword="enum" /> for validation and testing purposes.
        /// </summary>
        public enum EmptyEnum
        {
        }

        /// <summary>
        ///     Value that internally determines that an <see langword="enum" /> instance is empty/invalid, since enum items
        ///     defaultly
        ///     starts with zero.
        /// </summary>
        private const int InvalidEnumItem = 60000;

        //TODO: missing code
        //public static EnumRepo GetRepo(Enum item)
        //{
        //    return EnumRepo.Get(item);
        //}

        /// <summary>
        ///     Returns an instance of the provided <see langword="enum" /> type, with the stored value of
        ///     <see cref="InvalidEnumItem" /> .
        /// </summary>
        public static Enum GetEmpty(Type type)
        {
            type.ValidateEnumType();

            return (Enum)Enum.ToObject(type, InvalidEnumItem);
        }

        /// <summary>
        ///     Tests if the given <see langword="enum" /> <paramref name="item" />
        ///     is valid, throwing otherwise.
        /// </summary>
        public static void Validate(Enum item)
        {
            (item?.GetType()).ValidateEnumType();
        }
    }
}
