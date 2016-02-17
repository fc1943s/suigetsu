using NUnit.Framework;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Tests.Extensions
{
    [TestFixture]
    public class ArrayExtensionsTests : AssertionHelper
    {
        [SetUp]
        public void Initialize()
        {
        }

        [TearDown]
        public void Cleanup()
        {
        }

        [Test]
        public void ConvertTest()
        {
            var arr = new[] { 10, 20, 30 };
            var res = arr.Convert<byte>();
            Expect(res.Convert<int>(), EquivalentTo(arr));

            Expect(new[] { 33, 34 }.Convert<int>(), EquivalentTo(new[] { 33, 34 }));

            Expect(new[] { 'a', 'b' }.Convert<string>(), EquivalentTo(new[] { "a", "b" }));

            Expect(new[] { "a", "b" }.Convert<char>(), EquivalentTo(new[] { 'a', 'b' }));
        }
    }
}
