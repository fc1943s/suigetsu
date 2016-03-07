using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Suigetsu.Core.Common;
using Suigetsu.Core.Serialization;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="T:System.String" /> class.
    /// </summary>
    public static class StringExtensions
    {
        private static object FromJson(this string str, Type type,
                                       JsonSerializerSettingsWrapperParameters settings = null)
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

        public static int ToInt(this string text, NumberStyles style = NumberStyles.Integer) => int.Parse(text, style);

        public static long ToLong(this string text) => long.Parse(text);

        public static IList<int> HexToDigits(this string hex, int digitLength = StringUtils.DefaultHexDigitLength)
        {
            var bytes = new List<int>();

            for(var i = 0; i < hex.Length; i += digitLength)
            {
                if((hex.Length - i) >= digitLength)
                {
                    bytes.Add(hex.Substring(i, digitLength).Trim().ToInt(NumberStyles.HexNumber));
                }
            }

            return bytes;
        }

        public static string HexToString(this string hex, int digitLength)
            => new string(hex.HexToDigits(digitLength).ToArray().Convert<char>());

        public static string HexToString(this string hex) => hex.HexToString(StringUtils.DefaultHexDigitLength);

        public static IEnumerable<byte> HexToBytes(this string hex) => hex.HexToDigits(2).ToArray().Convert<byte>();

        private static IEnumerable<string> GraphemeClusters(this string str)
        {
            var enumerator = StringInfo.GetTextElementEnumerator(str);
            while(enumerator.MoveNext())
            {
                yield return (string)enumerator.Current;
            }
        }

        public static string Reverse(this string str)
            => string.Join(string.Empty, str.GraphemeClusters().Reverse().ToArray());

        public static string GetNumbers(this string text) => Regex.Replace(text, @"(\D)", string.Empty);

        public static string ToUtf8(this string text) => text.ToUtf8(Encoding.GetEncoding(1252));

        private static string ToUtf8(this string text, Encoding fromEncoding)
            => Encoding.UTF8.GetString(fromEncoding.GetBytes(text));

        public static string FixNewLine(this string text)
        {
            return Regex.Replace("", @"(\r\n?|\n)", Environment.NewLine);
        }
    }
}
