using System;

namespace StrokeForEgypt.Common
{
    public static class CurrentTime
    {
        public static DateTime Egypt(string timeZone = "Egypt Standard Time")
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id, timeZone);
        }

        public static DateTime UTC()
        {
            return DateTime.UtcNow;
        }
    }
}
