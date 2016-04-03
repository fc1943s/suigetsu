using System;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Util
{
    public static class DateTimeUtils
    {
        public static DateTime GetEpochTime() => new DateTime(1970, 1, 1);

        public static long GetTimestamp(bool seconds = false) => DateTime.Now.GetTimestamp(seconds);
    }
}
