using AutoMapper;

namespace AgileLabs.EfCore.PostgreSQL;

public class DatetimeMapping : Profile
{
    public DatetimeMapping()
    {
        CreateMap<DateTime, DateTimeOffset>().ConvertUsing<DateTime2DateTimeOffset>();
        CreateMap<DateTimeOffset, DateTime>().ConvertUsing<DateTime2DateTimeOffset>();
        CreateMap<DateTime?, DateTimeOffset?>().ConvertUsing<DateTime2DateTimeOffset>();
        CreateMap<DateTimeOffset?, DateTime?>().ConvertUsing<DateTime2DateTimeOffset>();

    }
}
