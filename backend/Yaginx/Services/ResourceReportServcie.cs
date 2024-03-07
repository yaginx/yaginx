using Yaginx.DomainModels.MonitorModels;

namespace Yaginx.Services
{
    /// <summary>
    /// 统计服务
    /// </summary>
    public class ResourceReportServcie
    {
        private readonly IMonitorInfoRepository _monitorInfoRepository;
        private readonly IResourceReportRepository _resourceReportRepository;

        public ResourceReportServcie(IMonitorInfoRepository monitorInfoRepository, IResourceReportRepository resourceReportRepository)
        {
            _monitorInfoRepository = monitorInfoRepository;
            _resourceReportRepository = resourceReportRepository;
        }

        public async Task MinutelyCheckAsync(DateTime nowTime)
        {
            var beginTime = nowTime.Date.AddHours(nowTime.Hour).AddMinutes(nowTime.Minute);
            var endTime = beginTime.AddMinutes(1);
            await CycleReportStatisticAsync(ReportCycleType.Minutely, beginTime, endTime);
            await Task.CompletedTask;
        }

        public async Task HourlyCheckAsync(DateTime nowTime)
        {
            var beginTime = nowTime.Date.AddHours(nowTime.Hour);
            var endTime = beginTime.AddHours(1);
            await CycleReportStatisticAsync(ReportCycleType.Hourly, beginTime, endTime);
            await Task.CompletedTask;
        }

        public async Task DailyCheckAsync(DateTime nowTime)
        {
            var beginTime = nowTime.Date;
            var endTime = beginTime.AddDays(1);
            await CycleReportStatisticAsync(ReportCycleType.Daily, beginTime, endTime);
            await Task.CompletedTask;
        }

        public async Task CycleReportStatisticAsync(ReportCycleType cycleType, DateTime beginTime, DateTime endTime)
        {
            switch (cycleType)
            {
                case ReportCycleType.Minutely:
                case ReportCycleType.Hourly:
                    await HourlyReportStatisticAsync(cycleType, beginTime, endTime);
                    break;
                case ReportCycleType.Daily:
                    await DailyReportStatisticAsync(beginTime, endTime);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 每日报表数据基于每小时的数据
        /// </summary>
        /// <param name="resourceUuid"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private async Task DailyReportStatisticAsync(DateTime beginTime, DateTime endTime)
        {
            //var monitorInfoFilterBuilder = Builders<ResourceReportEntity>.Filter;
            //var monitorInfoFilter = monitorInfoFilterBuilder.Empty;
            //monitorInfoFilter &= monitorInfoFilterBuilder.Eq(x => x.CycleType, ReportCycleType.Hourly);
            //monitorInfoFilter &= monitorInfoFilterBuilder.Gte(x => x.ReportTime, beginTime);
            //monitorInfoFilter &= monitorInfoFilterBuilder.Lt(x => x.ReportTime, endTime);

            //var houlyDataList = await _reportRep.SearchAsync(monitorInfoFilter);

            var houlyDataList = await _resourceReportRepository.SearchAsync(ReportCycleType.Hourly, beginTime, endTime);

            var resourceIds = houlyDataList.GroupBy(x => x.ResourceUuid).Select(x => x.Key);
            foreach (var item in resourceIds)
            {
                var currentRegionDatas = houlyDataList.Where(x => x.ResourceUuid == item);
                if (!currentRegionDatas.Any())
                {
                    continue;
                }

                var resourceReport = new ResourceReportModel()
                {
                    ResourceUuid = item,
                    CycleType = ReportCycleType.Daily,
                    ReportTime = beginTime,
                    RequestQty = currentRegionDatas.Sum(x => x.RequestQty),
                    CreateTime = DateTime.Now
                };

                resourceReport.Duration = AnalysisDailyDuration(currentRegionDatas.Where(x => x.Duration != null).SelectMany(x => x.Duration)).ToKeyValuePair();

                resourceReport.Spider = currentRegionDatas
                    .Where(x => x.Spider != null)
                    .SelectMany(x => x.Spider)
                    .GroupBy(x => x.Key)
                    .Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToList();
                resourceReport.Browser = currentRegionDatas
                    .Where(x => x.Browser != null)
                    .SelectMany(x => x.Browser)
                    .GroupBy(x => x.Key)
                    .Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToList();
                resourceReport.Os = currentRegionDatas
                    .Where(x => x.Os != null)
                    .SelectMany(x => x.Os)
                    .GroupBy(x => x.Key)
                    .Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToList();
                resourceReport.StatusCode = currentRegionDatas
                    .Where(x => x.StatusCode != null)
                    .SelectMany(x => x.StatusCode)
                    .GroupBy(x => x.Key)
                    .Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToList();

                await InsertOrUpdateResourceReport(resourceReport);
            }
        }

        public static Dictionary<string, long> AnalysisDailyDuration(IEnumerable<KeyValuePair<string, long>> durationList)
        {
            string[] ignoreKeys = new string[] { "avg", "max" };
            var avg = durationList.Where(x => x.Key == ignoreKeys[0]).Select(x => x.Value).Sum() / durationList.LongCount();
            var max = durationList.Where(x => x.Key == ignoreKeys[1]).Select(x => x.Value).Max(x => x);

            var result = durationList.Where(x => !ignoreKeys.Contains(x.Key))
                .GroupBy(x => x.Key)
                .Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value)))
                .ToDictionary(x => x.Key, y => y.Value);

            result.Add(ignoreKeys[0], avg);
            result.Add(ignoreKeys[1], max);
            return result;
        }

        private async Task HourlyReportStatisticAsync(ReportCycleType reportCycleType, DateTime beginTime, DateTime endTime)
        {
            var monitorInfoList = await ResourceMonitorInfoSearchCycleRangeRecords(beginTime, endTime);
            var hosts = monitorInfoList.SelectMany(x => x.Data).Select(x => x.Host).Distinct();

            foreach (var host in hosts)
            {
                var currentRegionMonitorInfos = monitorInfoList.Where(x => x.Data.Select(y => y.Host).Contains(host)).SelectMany(x => x.Data);

                if (!currentRegionMonitorInfos.Any())
                {
                    continue;
                }

                var resourceReport = new ResourceReportModel()
                {
                    ResourceUuid = host,
                    CycleType = reportCycleType,
                    ReportTime = beginTime,
                    RequestQty = currentRegionMonitorInfos.LongCount(),
                    CreateTime = DateTime.Now
                };

                resourceReport.Duration = AnlysisHourlyDuration(currentRegionMonitorInfos).ToKeyValuePair();
                resourceReport.Spider = AnlysisHourlySpider(currentRegionMonitorInfos).ToKeyValuePair();
                resourceReport.Browser = AnlysisHourlyBrowser(currentRegionMonitorInfos).ToKeyValuePair();
                resourceReport.Os = AnlysisHourlyOperationSystem(currentRegionMonitorInfos).ToKeyValuePair();
                resourceReport.StatusCode = AnlysisHourlyStatusCode(currentRegionMonitorInfos).ToKeyValuePair();

                await InsertOrUpdateResourceReport(resourceReport);
            }
        }

        private async Task InsertOrUpdateResourceReport(ResourceReportModel resourceReport)
        {
            //var filter = Builders<ResourceReportEntity>.Filter.Where(x => x.ResourceUuid == resourceReport.ResourceUuid
            //&& x.CycleType == resourceReport.CycleType
            //&& x.ReportTime == resourceReport.ReportTime);

            //var update = Builders<ResourceReportEntity>.Update
            //    .SetOnInsert(x => x.ResourceUuid, resourceReport.ResourceUuid)
            //    .SetOnInsert(x => x.CycleType, resourceReport.CycleType)
            //    .SetOnInsert(x => x.ReportTime, resourceReport.ReportTime)
            //    .Set(x => x.RequestQty, resourceReport.RequestQty)
            //    .Set(x => x.Duration, resourceReport.Duration)
            //    .Set(x => x.Spider, resourceReport.Spider)
            //    .Set(x => x.Browser, resourceReport.Browser)
            //    .Set(x => x.Os, resourceReport.Os)
            //    .Set(x => x.StatusCode, resourceReport.StatusCode)
            //    .Set(x => x.CreateTime, resourceReport.CreateTime);
            //var result = await _reportRep.Collection.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });
            await _resourceReportRepository.UpsertAsync(resourceReport);

        }

        /// <summary>
        /// Spider数据
        /// </summary>
        /// <param name="allData"></param>
        /// <returns></returns>
        private Dictionary<string, long> AnlysisHourlySpider(IEnumerable<MonitorInfo> allData)
        {
            var spiderRequestData = allData.Where(x => x.Device != null && x.Ua != null && x.Device.IsSpider);
            if (!spiderRequestData.Any())
                return null;

            var spiderResult = spiderRequestData.Select(x => x.Ua.Family).GroupBy(x => x).Select(x => KeyValuePair.Create(x.Key, x.LongCount()));
            var result = new Dictionary<string, long>(spiderResult);
            if (!spiderResult.Any())
                return null;

            result.Add("total", spiderRequestData.LongCount());
            return result;
        }

        /// <summary>
        /// 排除Spider之外的浏览器数据
        /// </summary>
        /// <param name="allData"></param>
        /// <returns></returns>
        private Dictionary<string, long> AnlysisHourlyBrowser(IEnumerable<MonitorInfo> allData)
        {
            var spiderRequestData = allData.Where(x => x.Device != null && x.Ua != null && !x.Device.IsSpider);
            if (!spiderRequestData.Any())
                return null;

            var spiderResult = spiderRequestData.Select(x => x.Ua.Family)
                .GroupBy(x => x)
                .Select(x => KeyValuePair.Create(x.Key, x.LongCount()));
            if (!spiderResult.Any())
                return null;

            var result = new Dictionary<string, long>(spiderResult);
            result.Add("total", spiderRequestData.LongCount());
            return result;
        }

        /// <summary>
        /// 分析OS
        /// </summary>
        /// <param name="allData"></param>
        /// <returns></returns>
        private Dictionary<string, long> AnlysisHourlyOperationSystem(IEnumerable<MonitorInfo> allData)
        {
            var osResult = allData.Where(x => x.Os != null)
                .Select(x => $"{x.Os.Family}-{x.Os.Major}").GroupBy(x => x).Select(x => KeyValuePair.Create(x.Key, x.LongCount()));
            if (!osResult.Any())
                return null;
            return new Dictionary<string, long>(osResult);
        }
        private Dictionary<string, long> AnlysisHourlyStatusCode(IEnumerable<MonitorInfo> allData)
        {
            var spiderResult = allData.Select(x => x.StatusCode).GroupBy(x => x).Select(x => KeyValuePair.Create(x.Key.ToString(), x.LongCount()));
            if (!spiderResult.Any())
                return null;
            return new Dictionary<string, long>(spiderResult);
        }
        private Dictionary<string, long> AnlysisHourlyDuration(IEnumerable<MonitorInfo> allData)
        {
            if (!allData.Any())
                return null;

            var result = new Dictionary<string, long>();
            result.Add("avg", Convert.ToInt32(allData.Sum(x => x.Duration) / allData.LongCount()));
            result.Add("max", allData.Max(x => x.Duration));
            result.Add("<=100", allData.Count(x => x.Duration <= 100));
            result.Add(">100 & <=500", allData.Count(x => x.Duration > 100 && x.Duration <= 500));
            result.Add(">500 & <=1000", allData.Count(x => x.Duration > 500 && x.Duration <= 1000));
            result.Add(">1000", allData.Count(x => x.Duration > 1000));
            return result;
        }

        private async Task<IList<ResourceMonitorInfo>> ResourceMonitorInfoSearchCycleRangeRecords(DateTime beginTime, DateTime endTime)
        {
            //var monitorInfoFilterBuilder = Builders<ResourceMonitorInfoEntity>.Filter;
            //var monitorInfoFilter = monitorInfoFilterBuilder.Empty;
            //monitorInfoFilter &= monitorInfoFilterBuilder.Gte(x => x.Timestamp, beginTime);
            //monitorInfoFilter &= monitorInfoFilterBuilder.Lt(x => x.Timestamp, endTime);
            //var monitorInfos = await _monitorInfoRep.SearchAsync(monitorInfoFilter);
            //return monitorInfos;

            return await _monitorInfoRepository.SearchAsync(beginTime, endTime);
        }
    }

    public static class DicExtension
    {
        public static List<KeyValuePair<TKey, TValue>> ToKeyValuePair<TKey, TValue>(this Dictionary<TKey, TValue> obj)
        {
            if (obj == null)
                return null;

            return obj.Select(x => new KeyValuePair<TKey, TValue>(x.Key, x.Value)).ToList();
        }
    }
}
