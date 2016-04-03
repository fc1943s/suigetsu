using System.Net;
using NLog;
using Suigetsu.Core.Logging;

namespace Suigetsu.Core.Net
{
    public static class WebUtils
    {
        private static readonly Logger Logger = LoggingManager.GetCurrentClassLogger();

        public static string DownloadString(string address)
        {
            Logger.Trace("Downloading address '{0}' as string.", address);

            return new WebClient().DownloadString(address);
        }
    }
}
