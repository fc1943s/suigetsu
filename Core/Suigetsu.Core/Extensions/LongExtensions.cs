using System;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Extensions
{
    public static class LongExtensions
    {
        public static DateTime GetDateTime(this long timestamp)
        {
            return TimeZoneInfo.ConvertTimeFromUtc
                (DateTimeUtils.GetEpochTime().AddMilliseconds(timestamp > 1000000000000 ? timestamp : timestamp * 1000),
                 TimeZoneInfo.Local);
        }
    }
}
