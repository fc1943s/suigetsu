using NUnit.Framework;
using Suigetsu.Core.Extensions;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Tests.Util
{
    [TestFixture]
    public class TypeUtilsTests : AssertionHelper
    {
        [Test]
        public void DefaultTest()
        {
            Expect(TypeUtils.Default<string>(), EqualTo(string.Empty));
            Expect(TypeUtils.Default<EnumUtils.EmptyEnum>(), EqualTo(default(EnumUtils.EmptyEnum).GetEmpty()));
            Expect(TypeUtils.Default<int>(), EqualTo(0));
        }
    }
}
