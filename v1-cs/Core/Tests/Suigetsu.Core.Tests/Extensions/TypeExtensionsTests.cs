using System.ComponentModel;
using NUnit.Framework;
using Suigetsu.Core.Extensions;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Tests.Extensions
{
    [TestFixture]
    public class TypeExtensionsTests : AssertionHelper
    {
        [Test]
        public void InTest()
        {
            Expect("Test".In("0", "Test", "1"));
            Expect("Test".In("0", "TEST", "1"), False);

            var arr = new[] { 0, 1, 2 };
            Expect(1.In(arr));
            Expect(100.In(arr), False);
        }

        [Test]
        public void ValidateEnumTypeTest()
        {
            Expect(() => typeof(TypeExtensionsTests).ValidateEnumType(), Throws.InstanceOf<InvalidEnumArgumentException>());
            Expect(() => typeof(EnumUtils.EmptyEnum).ValidateEnumType(), Throws.Nothing);
        }
    }
}
