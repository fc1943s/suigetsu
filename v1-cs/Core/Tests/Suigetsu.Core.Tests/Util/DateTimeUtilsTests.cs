using NUnit.Framework;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Tests.Util
{
    [TestFixture]
    public class DateTimeUtilsTests : AssertionHelper
    {
        [Test]
        public void TimestampTest()
        {
            var timestamp = DateTimeUtils.GetTimestamp(true);
            Expect(timestamp, LessThan(10000000000));
        }
    }
}
