namespace ITHelpDesk.Application.Common.Helpers
{
    public static class DateHelper
    {
        public static string FormatCairoDate(DateTime dateTime)
        {
            var cairoTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

            var cairoDate =
                TimeZoneInfo.ConvertTimeFromUtc(dateTime, cairoTimeZone);

            return cairoDate.ToString("dd MMM yyyy, hh:mm tt");
        }
        public static DateTime ToCairoDate(DateTime dateTime)
        {
            var cairoTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, cairoTimeZone);
        }
        public static DateTime ToUtcDate(DateTime dateTime)
        {
            return dateTime.Kind switch
            {
                DateTimeKind.Utc => dateTime,
                DateTimeKind.Local => dateTime.ToUniversalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime(),
                _ => dateTime.ToUniversalTime()
            };
        }
    }
}
