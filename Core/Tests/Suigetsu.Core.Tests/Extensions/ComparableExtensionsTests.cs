using NUnit.Framework;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Tests.Extensions
{
    [TestFixture]
    public class ComparableExtensionsTests : AssertionHelper
    {
        [Test]
        public void IsBetweenTest()
        {
            Expect(150f.IsBetween(100.0f, 200.5f));

            Expect(100.IsBetween(100, 200));
            Expect(200.IsBetween(100, 200));

            Expect(99.99d.IsBetween(100d, 200d), False);
            Expect(201.IsBetween(100, 200), False);

            Expect('B'.IsBetween('A', 'C'));
        }
    }
}
