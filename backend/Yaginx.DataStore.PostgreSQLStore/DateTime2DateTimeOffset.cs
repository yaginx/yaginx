using AutoMapper;

namespace WoLabs.AutoMapper;

public class DateTime2DateTimeOffset : ITypeConverter<DateTime, DateTimeOffset>, ITypeConverter<DateTimeOffset, DateTime>, ITypeConverter<DateTime?, DateTimeOffset?>, ITypeConverter<DateTimeOffset?, DateTime?>
{
    public DateTimeOffset Convert(DateTime source, DateTimeOffset destination, ResolutionContext context)
    {
        destination = new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(source));
        return destination;
    }

    public DateTime Convert(DateTimeOffset source, DateTime destination, ResolutionContext context)
    {
        destination = source.UtcDateTime.ToLocalTime();
        return destination;
    }

    public DateTimeOffset? Convert(DateTime? source, DateTimeOffset? destination, ResolutionContext context)
    {
        if (!source.HasValue)
            return null;

        destination = new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(source.Value));
        return destination;
    }

    public DateTime? Convert(DateTimeOffset? source, DateTime? destination, ResolutionContext context)
    {
        if (!source.HasValue)
            return null;

        destination = source.Value.UtcDateTime.ToLocalTime();
        return destination;
    }
}
