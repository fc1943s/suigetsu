using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Suigetsu.Core.Common;
using Suigetsu.Core.Configuration;
using Suigetsu.Core.Cryptography;
using Suigetsu.Core.Serialization;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="T:System.String" /> class.
    /// </summary>
    public static class StringExtensions
    {
        private static object FromJson(this string str, Type type, JsonSerializerSettingsWrapperParameters settings = null)
            => JsonConvert.DeserializeObject(str, type, new JsonSerializerSettingsWrapper(settings));

        /// <summary>
        ///     Deserializes the given json string into a <see langword="object" /> of the given
        ///     <typeparamref name="T" /> type.
        /// </summary>
        public static T FromJson<T>(this string str, JsonSerializerSettingsWrapperParameters settings = null)
            => (T)str.FromJson(typeof(T), settings);

        /// <summary>
        ///     Tests if the <paramref name="str" /> is empty.
        /// </summary>
        public static bool IsEmpty(this string str) => string.IsNullOrEmpty(str);

        /// <summary>
        ///     Wraps the <paramref name="str" /> with the given <paramref name="edge" />.
        /// </summary>
        public static string Wrap(this string str, string edge) => Wrap(str, edge, edge);

        /// <summary>
        ///     Wraps the <paramref name="str" /> with the given <paramref name="before" /> and
        ///     <paramref name="after" /> parameters.
        /// </summary>
        public static string Wrap(this string str, string before, string after) => before + str + after;

        /// <summary>
        ///     Converts the <paramref name="text" /> to its int equivalent.
        /// </summary>
        public static int ToInt(this string text, NumberStyles style = NumberStyles.Integer) => int.Parse(text, style);

        /// <summary>
        ///     Converts the <paramref name="text" /> to its Int64 equivalent.
        /// </summary>
        public static long ToLong(this string text) => long.Parse(text);

        /// <summary>
        ///     Breaks the given <paramref name="hex" /> string into digits.
        /// </summary>
        public static IEnumerable<int> HexToDigits(this string hex, int digitLength = StringUtils.DefaultHexDigitLength)
        {
            for(var i = 0; i < hex.Length; i += digitLength)
            {
                if((hex.Length - i) >= digitLength)
                {
                    yield return hex.Substring(i, digitLength).Trim().ToInt(NumberStyles.HexNumber);
                }
            }
        }

        /// <summary>
        ///     Breaks the given <paramref name="hex" /> string into chars and join them into a new string.
        /// </summary>
        public static string HexToString(this string hex, int digitLength = StringUtils.DefaultHexDigitLength)
            => new string(hex.HexToDigits(digitLength).ToArray().Convert<char>());

        /// <summary>
        ///     Breaks the given <paramref name="hex" /> string into bytes.
        /// </summary>
        public static IEnumerable<byte> HexToBytes(this string hex) => hex.HexToDigits(2).ToArray().Convert<byte>();

        private static IEnumerable<string> GetGraphemeClusters(this string str)
        {
            var enumerator = StringInfo.GetTextElementEnumerator(str);
            while(enumerator.MoveNext())
            {
                yield return (string)enumerator.Current;
            }
        }

        public static DateTime ToDateTime(this string text)
            => DateTime.ParseExact(text, Settings.Get<Settings>().DefaultDateFormat, null);

        public static string GetMd5(this string text, int repeatCount = 1) => Md5.FromString(text, repeatCount);

        /// <summary>
        ///     Reverses a text respecting unicode characters.
        /// </summary>
        public static string Reverse(this string str) => string.Join(string.Empty, str.GetGraphemeClusters().Reverse().ToArray());

        /// <summary>
        ///     Extracts the digits from the <paramref name="text" />.
        /// </summary>
        public static string GetNumbers(this string text) => Regex.Replace(text, @"(\D)", string.Empty);

        /// <summary>
        ///     Converts the <paramref name="text" /> to the UTF-8 encoding.
        /// </summary>
        public static string ToUtf8(this string text) => text.ToUtf8(Encoding.GetEncoding(1252));

        private static string ToUtf8(this string text, Encoding fromEncoding)
            => Encoding.UTF8.GetString(fromEncoding.GetBytes(text));

        /// <summary>
        ///     Normalizes the text's line breaks between different environments.
        /// </summary>
        public static string FixNewLine(this string text) => Regex.Replace(text, @"(\r\n?|\n)", Environment.NewLine);
    }
}
