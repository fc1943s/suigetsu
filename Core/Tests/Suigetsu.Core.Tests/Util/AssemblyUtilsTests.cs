using NUnit.Framework;
using Suigetsu.Core.Common;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Tests.Util
{
    [TestFixture]
    public class AssemblyUtilsTests : AssertionHelper
    {
        [Test]
        public void GetCallingTypeTest()
        {
            Expect(AssemblyUtils.GetCallingType(), EqualTo(GetType()));
        }

        [Test]
        public void GetCallingTypeNameTest()
        {
            Expect(AssemblyUtils.GetCallingTypeName(), EqualTo(GetType().FullName));
        }

        [Test]
        public void GetEntryAssemblyTest()
        {
            Expect(AssemblyUtils.GetEntryAssembly(), EqualTo(Testing.GetTestAssembly()));
        }
    }
}
