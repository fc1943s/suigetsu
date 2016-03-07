

using NLog;
using NUnit.Framework;
using Suigetsu.Core.Logging;

namespace Suigetsu.Core.Tests.Logging
{
    [TestFixture]
    public class LoggingManagerTests : AssertionHelper
    {
        [Test]
        public void GetGlobalLoggerTest()
        {
            var logger = LoggingManager.GetGlobalLogger();
            Expect(logger.IsEnabled(LogLevel.Trace));
            Expect(logger.Name, EqualTo(typeof(LoggingManager).FullName));
        }
    }
}
