using NUnit.Framework;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests : AssertionHelper
    {
        [Test]
        public void FormatWithTest()
        {
            Expect("A{0}{1}D".FormatWith("B", "C"), EqualTo("ABCD"));
        }
    }
}
