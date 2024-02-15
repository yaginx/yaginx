using AgileLabs;

namespace Yaginx.DomainModels;

public class TimePeriod
{
    public static PeriodInfo GetCurrentPeriod(SeqNoResetPeriod resetPeriod, DateTime dbTime)
    {
        PeriodInfo periodInfo = new PeriodInfo()
        {
            Period = resetPeriod,
            RequestTs = dbTime.GetEpochSeconds()
        };
        long result = -1;
        switch (resetPeriod)
        {
            case SeqNoResetPeriod.PerMinutes:
                result = new DateTime(dbTime.Year, dbTime.Month, dbTime.Day, dbTime.Hour, dbTime.Minute, 0, DateTimeKind.Utc).GetEpochSeconds();
                periodInfo.Items.Add("YEAR", dbTime.Year);
                periodInfo.Items.Add("MONTH", dbTime.Month);
                periodInfo.Items.Add("DAY", dbTime.Day);
                periodInfo.Items.Add("HOUR", dbTime.Hour);
                periodInfo.Items.Add("MINUTE", dbTime.Minute);
                periodInfo.Items.Add("PERIOD_NO", dbTime.Minute);//计数周期从1开始
                break;
            case SeqNoResetPeriod.FiveMinutes:
                result = new DateTime(dbTime.Year, dbTime.Month, dbTime.Day, dbTime.Hour, dbTime.Minute / 5 * 5, 0, DateTimeKind.Utc).GetEpochSeconds();
                periodInfo.Items.Add("YEAR", dbTime.Year);
                periodInfo.Items.Add("MONTH", dbTime.Month);
                periodInfo.Items.Add("DAY", dbTime.Day);
                periodInfo.Items.Add("HOUR", dbTime.Hour);
                periodInfo.Items.Add("PERIOD_NO", ((dbTime.Minute / 5) + 1));//计数周期从1开始
                break;
            case SeqNoResetPeriod.TenMinutes:
                result = new DateTime(dbTime.Year, dbTime.Month, dbTime.Day, dbTime.Hour, dbTime.Minute / 10 * 10, 0, DateTimeKind.Utc).GetEpochSeconds();
                periodInfo.Items.Add("YEAR", dbTime.Year);
                periodInfo.Items.Add("MONTH", dbTime.Month);
                periodInfo.Items.Add("DAY", dbTime.Day);
                periodInfo.Items.Add("HOUR", dbTime.Hour);
                periodInfo.Items.Add("PERIOD_NO", ((dbTime.Minute / 10) + 1));//计数周期从1开始
                break;
            case SeqNoResetPeriod.Quarter:
                result = new DateTime(dbTime.Year, dbTime.Month, dbTime.Day, dbTime.Hour, dbTime.Minute / 15 * 15, 0, DateTimeKind.Utc).GetEpochSeconds();
                periodInfo.Items.Add("YEAR", dbTime.Year);
                periodInfo.Items.Add("MONTH", dbTime.Month);
                periodInfo.Items.Add("DAY", dbTime.Day);
                periodInfo.Items.Add("HOUR", dbTime.Hour);
                periodInfo.Items.Add("PERIOD_NO", ((dbTime.Minute / 15) + 1));//计数周期从1开始
                break;
            case SeqNoResetPeriod.HalfHour:
                result = new DateTime(dbTime.Year, dbTime.Month, dbTime.Day, dbTime.Hour, dbTime.Minute / 30 * 30, 0, DateTimeKind.Utc).GetEpochSeconds();
                periodInfo.Items.Add("YEAR", dbTime.Year);
                periodInfo.Items.Add("MONTH", dbTime.Month);
                periodInfo.Items.Add("DAY", dbTime.Day);
                periodInfo.Items.Add("HOUR", dbTime.Hour);
                periodInfo.Items.Add("PERIOD_NO", ((dbTime.Minute / 30) + 1));//计数周期从1开始
                break;
            case SeqNoResetPeriod.Hourly:
                result = new DateTime(dbTime.Year, dbTime.Month, dbTime.Day, dbTime.Hour, 0, 0, DateTimeKind.Utc).GetEpochSeconds();
                periodInfo.Items.Add("YEAR", dbTime.Year);
                periodInfo.Items.Add("MONTH", dbTime.Month);
                periodInfo.Items.Add("DAY", dbTime.Day);
                periodInfo.Items.Add("HOUR", dbTime.Hour);
                periodInfo.Items.Add("PERIOD_NO", dbTime.Hour);
                break;
            case SeqNoResetPeriod.Daily:
                result = new DateTime(dbTime.Year, dbTime.Month, dbTime.Day, 0, 0, 0, DateTimeKind.Utc).GetEpochSeconds();
                periodInfo.Items.Add("YEAR", dbTime.Year);
                periodInfo.Items.Add("MONTH", dbTime.Month);
                periodInfo.Items.Add("DAY", dbTime.Day);
                periodInfo.Items.Add("PERIOD_NO", dbTime.Day);
                break;
            case SeqNoResetPeriod.Weekly:
                //var cal = CultureInfo.CurrentCulture.Calendar;
                //var (year, week) = GetWeekNo(dbTime, cal, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                //var firstDayWeek = FirstDateOfWeek(year, week, cal, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                //result = firstDayWeek.GetEpochSeconds();

                //periodInfo.Items.Add("YEAR", year);
                //periodInfo.Items.Add("WEEK", week);
                //periodInfo.Items.Add("PERIOD_NO", week);
                break;
            case SeqNoResetPeriod.Monthly:
                result = new DateTime(dbTime.Year, dbTime.Month, 0, 0, 0, 0, DateTimeKind.Utc).GetEpochSeconds();
                periodInfo.Items.Add("YEAR", dbTime.Year);
                periodInfo.Items.Add("MONTH", dbTime.Month);
                periodInfo.Items.Add("PERIOD_NO", dbTime.Month);
                break;
            case SeqNoResetPeriod.Yearly:
                result = new DateTime(dbTime.Year, 0, 0, 0, 0, 0, DateTimeKind.Utc).GetEpochSeconds();
                periodInfo.Items.Add("YEAR", dbTime.Year);
                periodInfo.Items.Add("PERIOD_NO", dbTime.Year);
                break;
            case SeqNoResetPeriod.Never:
                break;
            default:
                break;
        }
        periodInfo.PeriodTs = result;
        return periodInfo;
    }
}
