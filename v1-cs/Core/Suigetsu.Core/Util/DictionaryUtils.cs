using System.Collections.Generic;

namespace Suigetsu.Core.Util
{
    public static class DictionaryUtils
    {
        public static KeyValuePair<TKey, TValue> CreatePair<TKey, TValue>(TKey key, TValue value)
            => new KeyValuePair<TKey, TValue>(key, value);
    }
}
