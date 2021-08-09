using System.Collections.Generic;
using NUnit.Framework;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Tests.Extensions
{
    [TestFixture]
    public class DictionaryExtensionsTests : AssertionHelper
    {
        [Test]
        public void GetTest()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("b", "b");
            Expect(dict.Get("a"), EqualTo(string.Empty));
            Expect(dict.Get("b"), EqualTo("b"));
        }
    }
}
