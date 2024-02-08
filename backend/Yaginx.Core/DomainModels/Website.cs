using AgileLabs;
using System.Globalization;

namespace Yaginx.DomainModels
{
    /// <summary>
    /// 站点
    /// </summary>
    public class Website
    {
        public long? Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 默认的主机头
        /// </summary>
        public string DefaultHost { get; set; }

        /// <summary>
        /// 默认的转发地址
        /// </summary>
        public string DefaultDestination { get; set; }

        public List<WebsiteHostItem> Hosts { get; set; }
        public List<WebsiteProxyRuleItem> ProxyRules { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public interface IWebsiteRepository
    {
        List<Website> Search();
        void Add(Website website);
        void Update(Website website);
        Website Get(long id);
        Website GetByName(string name);
    }

    public class HostTraffic
    {
        public long Id { get; set; }
        public string HostName { get; set; }
        public long Period { get; set; }
        public long RequestCounts { get; set; }
        public long InboundBytes { get; set; }
        public long OutboundBytes { get; set; }
    }

    public interface IHostTrafficRepository
    {
        void Upsert(HostTraffic hostTraffic);
        List<HostTraffic> Search();
        List<HostTraffic> Search(string hostName);
    }
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
    public class PeriodInfo
    {
        public long RequestTs { get; set; }
        public long PeriodTs { get; set; }
        public Dictionary<string, object> Items { get; set; } = new Dictionary<string, object>();
        public SeqNoResetPeriod Period { get; set; }
    }
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
}
