using AutoMapper;
using WoLabs.AutoMapper;

namespace Yaginx.DataStore.PostgreSQLStore;

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
