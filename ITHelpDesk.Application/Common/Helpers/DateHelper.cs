namespace ITHelpDesk.Application.Common.Helpers
{
    public static class DateHelper
    {
        private static readonly TimeZoneInfo CairoTimeZone = GetCairoTimeZone();

        private static TimeZoneInfo GetCairoTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo"); // Linux/.NET 6+, used by Azure
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"); // Windows fallback
            }
        }

        public static DateTime ToUtcTime(DateTime dateTime)
        {
            // Always treat incoming "unspecified" datetime-local values as Cairo time,
            // never as whatever the SERVER happens to think "Local" means.
            var unspecified = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, CairoTimeZone);
        }

        public static DateTime ToCairoTime(DateTime utcDateTime)
        {
            var utc = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(utc, CairoTimeZone);
        }

        public static string FormatCairoTime(DateTime utcDateTime)
        {
            return ToCairoTime(utcDateTime).ToString("dd MMM yyyy, hh:mm tt");
        }
    }
}
