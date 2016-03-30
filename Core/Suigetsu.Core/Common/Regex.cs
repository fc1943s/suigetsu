using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Common
{
    /// <summary>
    ///     Utility methods for regular expressions.
    /// </summary>
    public static class Regex
    {
        /// <summary>
        ///     Gets all the captures from all groups of the matched <paramref name="text" />, optionally replacing
        ///     them with the result of the given lambda.
        /// </summary>
        public static IList<IList<string>> MatchAll
            (string text, string pattern, Func<string, string> replace, out string replacedText)
        {
            //TODO: return nested enumerable. (test with a big text)

            var res = new List<IList<string>>();
            var reg = new System.Text.RegularExpressions.Regex
                (pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            replacedText = string.Empty;
            var replaceOffset = 0;

            var match = reg.Match(text);

            while(match.Success)
            {
                var curr = new List<string>();
                res.Add(curr);

                for(var i = 1; i < match.Groups.Count; i++)
                {
                    foreach(Capture v in match.Groups[i].Captures)
                    {
                        var block = v.Value;
                        if(replace != null)
                        {
                            block = replace(block);
                            replacedText += text.Substring(replaceOffset, v.Index - replaceOffset) + block;
                            replaceOffset = v.Index + v.Length;
                        }
                        curr.Add(block);
                    }
                }

                match = match.NextMatch();
            }

            replacedText += text.Substring(replaceOffset);

            return res;
        }

        /// <summary>
        ///     Gets all the captures from all groups of the matched <paramref name="text" />, optionally replacing
        ///     them with the result of the given lambda.
        /// </summary>
        public static IList<IList<string>> MatchAll(string text, string pattern, Func<string, string> replace = null)
        {
            string replacedText;
            return MatchAll(text, pattern, replace, out replacedText);
        }

        private static string Replace
            (string text, string pattern, Func<string, string> replace, out IList<IList<string>> captureList)
        {
            string replacedText;
            captureList = MatchAll(text, pattern, replace, out replacedText);
            return replacedText;
        }

        /// <summary>
        ///     Replaces all the captures from every group of the matched <paramref name="text" /> with the
        ///     result of the replace lambda.
        /// </summary>
        public static string Replace(string text, string pattern, Func<string, string> replace)
        {
            IList<IList<string>> captureList;
            return Replace(text, pattern, replace, out captureList);
        }

        /// <summary>
        ///     Replaces all the captures from every group of the matched <paramref name="text" /> with the
        ///     <paramref name="replace" /> parameter.
        /// </summary>
        public static string Replace(string text, string pattern, string replace)
            => Replace(text, pattern, x => replace);

        /// <summary>
        ///     Gets all the captures from the first group of the matched <paramref name="text" />.
        /// </summary>
        public static IList<string> Match(string text, string pattern)
            => MatchAll(text, pattern).FirstOrDefault(() => new List<string>());

        /// <summary>
        ///     Gets the first capture from the first group of the matched <paramref name="text" />.
        /// </summary>
        public static string MatchFirst(string text, string pattern)
            => Match(text, pattern).FirstOrDefault(() => string.Empty);

        /// <summary>
        ///     Tests if the given <paramref name="text" /> contains any capture.
        /// </summary>
        public static bool HasMatch(string text, string pattern) => !Match(text, pattern).IsEmpty();
    }
}
