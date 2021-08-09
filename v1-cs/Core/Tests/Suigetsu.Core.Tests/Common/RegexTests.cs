using NUnit.Framework;
using Suigetsu.Core.Common;

namespace Suigetsu.Core.Tests.Common
{
    [TestFixture]
    public class RegexTests : AssertionHelper
    {
        [Test]
        public void MatchTest()
        {
            Expect
                (Regex.MatchAll("One car red car blue car q", @"(\w+)\s+(car)"),
                 EquivalentTo(new[] { new[] { "One", "car" }, new[] { "red", "car" }, new[] { "blue", "car" } }));

            string replacedText;
            Expect
                (Regex.MatchAll("One car red car blue car q", @"(\w+)\s+(car)", x => x == "car" ? "x" : x, out replacedText),
                 EquivalentTo(new[] { new[] { "One", "x" }, new[] { "red", "x" }, new[] { "blue", "x" } }));
            Expect(replacedText, EqualTo("One x red x blue x q"));

            Expect(Regex.Replace("ABCDE1", @"([C\d])", "_"), EqualTo("AB_DE_"));

            Expect(Regex.Replace("ABCDE1", @"([C\d])", x => x == "1" ? "2" : x), EqualTo("ABCDE2"));

            Expect(Regex.HasMatch("ABCDE", @"([\d])"), False);

            Expect(Regex.Match("ABCDE1", @"([\d])")[0], EqualTo("1"));

            Expect(Regex.MatchFirst("ABCDE1", @"([\D])"), EqualTo("A"));

            Expect(Regex.MatchFirst("ABCDE", @"([\d])"), EqualTo(string.Empty));
        }
    }
}
