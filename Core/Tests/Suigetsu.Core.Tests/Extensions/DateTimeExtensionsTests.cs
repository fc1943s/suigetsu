using System;
using NUnit.Framework;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Tests.Extensions
{
    [TestFixture]
    public class DateTimeExtensionsTests : AssertionHelper
    {
        [Test]
        public void FormatDefaultTest()
        {
            var date = DateTime.Now;
            Expect(date.FormatDefault().ToDateTime(), EqualTo(date.TrimMilliseconds()));
        }

        [Test]
        public void TimestampTest()
        {
            var timestamp = DateTime.Now.GetTimestamp();

            Expect(timestamp, EqualTo(timestamp.GetDateTime().GetTimestamp()));
            Expect(timestamp, GreaterThan(1410441386000));
        }
    }
}
