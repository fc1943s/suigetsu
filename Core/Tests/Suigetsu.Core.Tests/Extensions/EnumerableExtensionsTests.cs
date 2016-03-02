using NUnit.Framework;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Tests.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsTests : AssertionHelper
    {
        [Test]
        public void SplitTest()
        {
            var list = new[] { 1, 2, 3, 4, 5 };

            Expect(list.Split(new[] { 2, 3 }), EquivalentTo(new[] { new[] { 1 }, new[] { 4, 5 } }));

            Expect(list.Split(new[] { 5 }), EquivalentTo(new[] { new[] { 1, 2, 3, 4 } }));

            Expect(list.Split(new[] { 4, 5 }), EquivalentTo(new[] { new[] { 1, 2, 3 } }));

            Expect(list.Split(new int[] { }), EquivalentTo(new[] { new[] { 1, 2, 3, 4, 5 } }));
        }
    }
}
