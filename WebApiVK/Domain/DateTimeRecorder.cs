using WebApiVK.Interfaces;

namespace WebApiVK.Domain;

public class DateTimeRecorder : IDateTimeRecorder
{
    public readonly TimeZoneInfo ZoneForTime = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

    public DateTime GetCurrentDateTime()
    {
        var currentTime = DateTime.Now.ToUniversalTime();
        DateTime zoneTime = TimeZoneInfo.ConvertTime(currentTime, ZoneForTime);
        var dateTime = TimeZoneInfo.ConvertTimeToUtc(zoneTime, ZoneForTime);
        return dateTime;
    }
}