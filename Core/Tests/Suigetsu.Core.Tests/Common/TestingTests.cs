using NUnit.Framework;
using Suigetsu.Core.Common;

namespace Suigetsu.Core.Tests.Common
{
    [TestFixture]
    public class TestingTests : AssertionHelper
    {
        [Test]
        public void GetTestAssemblyTest()
        {
            Expect(Testing.GetTestAssembly(), EqualTo(GetType().Assembly));
        }

        [Test]
        public void IsTestRunningTest()
        {
            Expect(Testing.IsTestRunning());
        }
    }
}
