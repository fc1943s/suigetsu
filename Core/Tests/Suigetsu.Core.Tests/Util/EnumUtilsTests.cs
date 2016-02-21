using System.ComponentModel;
using NUnit.Framework;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Tests.Util
{
    [TestFixture]
    public class EnumUtilsTests : AssertionHelper
    {
        //TODO: missing code
        //[Test]
        //public void EnumRepoTest()
        //{
        //    Expect(EnumUtils.GetRepo(null).Id, Empty);
        //}

        [Test]
        public void ValidateEnumTest()
        {
            Expect(() => EnumUtils.Validate(null), Throws.InstanceOf<InvalidEnumArgumentException>());
            Expect(() => EnumUtils.Validate(EnumUtils.DummyEnum.DummyEnumItem), Throws.Nothing);
        }
    }
}
