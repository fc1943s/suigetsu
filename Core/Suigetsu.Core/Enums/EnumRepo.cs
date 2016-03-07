using System;
using Suigetsu.Core.Extensions;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Enums
{
    public class EnumRepo : BaseEnumRepo
    {
        private readonly string _id;

        public EnumRepo(string id, string desc, params object[] data) : base(data)
        {
            _id = id ?? string.Empty;
            Desc = desc ?? string.Empty;
        }

        public EnumRepo(string id, params object[] data) : this(id, string.Empty, data)
        {
        }

        public EnumRepo() : this(string.Empty)
        {
        }

        public string Id
            => _id.IsEmpty() && !Equals(EnumUtils.GetEmpty(EnumItem.GetType()), EnumItem) ? EnumItem.ToString() : _id;

        public string Desc { get; }

        public T Arg<T>(int index)
        {
            if(index < 0 || index >= Data.Length)
            {
                return TypeUtils.Default<T>();
            }
            return (T)Data[index];
        }

        public EnumRepo ArgAsRepo(int index) => Get((Enum)Data[index]);

        public static EnumRepo Get<T>(T item) where T : struct, IComparable, IFormattable, IConvertible
            => GetRepo<T, EnumRepo>(item);

        public static EnumRepo Get(Enum item) => GetRepo<EnumRepo>(item);

        public static Enum EnumById(Type type, string id) => FindEnum<EnumRepo>(type, x => x.Id == id).EnumItem;

        public static T EnumById<T>(string id) where T : struct, IComparable, IFormattable, IConvertible
            => (T)(object)EnumById(typeof(T), id);
    }
}
