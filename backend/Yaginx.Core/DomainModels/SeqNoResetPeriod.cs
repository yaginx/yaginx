namespace Yaginx.DomainModels;

public enum SeqNoResetPeriod
{
    PerMinutes = 1,
    FiveMinutes = 2,
    TenMinutes = 3,
    Quarter = 4,
    HalfHour = 5,
    Hourly = 6,
    Daily = 11,
    Weekly = 12,
    Monthly = 13,
    Yearly = 14,
    Never = 99
}
