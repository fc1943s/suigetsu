using NUnit.Framework;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Tests.Extensions
{
    [TestFixture]
    public class ByteExtensionsTests : AssertionHelper
    {
        [Test]
        public void ByteArrayToStringTest()
        {
            Expect(new byte[] { 65, 63 }.ByteArrayToString(), EqualTo("413f"));
        }
    }
}
