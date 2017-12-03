using System;
using TimeZoneConverter;

namespace BotCommon
{
    public class TimeUtils
    {
        public static DateTime ToRealmTimeZone(DateTime origDateTime)
        {
            return TimeZoneInfo.ConvertTime(origDateTime, TZConvert.GetTimeZoneInfo("CET"));
        }
    }
}