using System;
using Suigetsu.Core.Configuration;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Extensions
{
    public static class DateTimeExtensions
    {
        private static TimeSpan GetEpochTimeSpan(this DateTime date) => date.Subtract(DateTimeUtils.GetEpochTime());

        public static long GetTimestamp(this DateTime date, bool seconds = false)
        {
            var timeSpan = date.ToUniversalTime().GetEpochTimeSpan();
            return (long)(seconds ? timeSpan.TotalSeconds : timeSpan.TotalMilliseconds);
        }

        public static string FormatDefault(this DateTime date) => date.ToString(Settings.Get<Settings>().DefaultDateFormat);

        public static DateTime TrimMilliseconds(this DateTime dt)
            => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0);
    }
}
