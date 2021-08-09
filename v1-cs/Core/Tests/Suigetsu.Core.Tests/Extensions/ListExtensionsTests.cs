using System.Linq;
using NUnit.Framework;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Tests.Extensions
{
    [TestFixture]
    public class ListExtensionsTests : AssertionHelper
    {
        [Test]
        public void FastReverseTest()
        {
            var data = new[] { 1, 2, 3, 4, 5 };
            Expect(data.FastReverse(), EquivalentTo(data.Reverse()));
            Expect(data, EquivalentTo(data.FastReverse().Reverse()));
        }

        [Test]
        public void IsEmptyTest()
        {
            Expect(new object[0].IsEmpty());
            Expect(new[] { 10 }.IsEmpty(), False);
        }
    }
}
