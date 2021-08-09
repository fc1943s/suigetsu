using System.Collections.Generic;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
            => dict.ContainsKey(key) ? dict[key] : TypeUtils.Default<TValue>();
    }
}
