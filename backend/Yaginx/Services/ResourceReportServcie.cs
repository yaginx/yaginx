﻿using MongoDB.Driver;
using Yaginx.DataStore.MongoStore;
using Yaginx.DataStore.MongoStore.Entities;

namespace Yaginx.Services
{
    /// <summary>
    /// 统计服务
    /// </summary>
    public class ResourceReportServcie
    {
        private readonly IAppNoSqlBaseRepository<ResourceEntity> _resourceRep;
        private readonly IAppNoSqlBaseRepository<ResourceSessionEntity> _sessionRep;
        private readonly IAppNoSqlBaseRepository<ResourceMonitorInfoEntity> _monitorInfoRep;
        private readonly IAppNoSqlBaseRepository<ResourceReportEntity> _reportRep;

        public ResourceReportServcie(IAppNoSqlBaseRepository<ResourceEntity> resourceRep,
            IAppNoSqlBaseRepository<ResourceSessionEntity> sessionRep,
            IAppNoSqlBaseRepository<ResourceMonitorInfoEntity> monitorInfoRep,
            IAppNoSqlBaseRepository<ResourceReportEntity> reportRep
            )
        {
            _resourceRep = resourceRep;
            _sessionRep = sessionRep;
            _monitorInfoRep = monitorInfoRep;
            _reportRep = reportRep;
        }
        public async Task HourlyCheckAsync()
        {
            var nowTime = DateTime.Now;
            var beginTime = nowTime.Date.AddHours(nowTime.Hour - 1);
            var endTime = beginTime.AddHours(1);
            //var allResources = await _resourceRep.SearchAsync(Builders<ResourceEntity>.Filter.Empty);
            //foreach (var resource in allResources)
            //{

            //}
            await CycleReportStatisticAsync("yaginx", ReportCycleType.Hourly, beginTime, endTime);
            await Task.CompletedTask;
        }

        public async Task DailyCheckAsync()
        {
            var nowTime = DateTime.Now;
            var beginTime = nowTime.AddHours(-1).Date;
            var endTime = beginTime.AddDays(1);
            var allResources = await _resourceRep.SearchAsync(Builders<ResourceEntity>.Filter.Empty);
            //foreach (var resource in allResources)
            //{
            //    await CycleReportStatisticAsync(resource.ResourceUuid, ReportCycleType.Daily, beginTime, endTime);
            //}
            await CycleReportStatisticAsync("yaginx", ReportCycleType.Daily, beginTime, endTime);
            await Task.CompletedTask;
        }

        public async Task CycleReportStatisticAsync(string resourceUuid, ReportCycleType cycleType, DateTime beginTime, DateTime endTime)
        {
            switch (cycleType)
            {
                case ReportCycleType.Hourly:
                    await HourlyReportStatisticAsync(resourceUuid, beginTime, endTime);
                    break;
                case ReportCycleType.Daily:
                    await DailyReportStatisticAsync(resourceUuid, beginTime, endTime);
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
        private async Task DailyReportStatisticAsync(string resourceUuid, DateTime beginTime, DateTime endTime)
        {
            var monitorInfoFilterBuilder = Builders<ResourceReportEntity>.Filter;
            var monitorInfoFilter = monitorInfoFilterBuilder.Eq(x => x.ResourceUuid, resourceUuid);
            monitorInfoFilter &= monitorInfoFilterBuilder.Eq(x => x.CycleType, ReportCycleType.Hourly);
            monitorInfoFilter &= monitorInfoFilterBuilder.Gte(x => x.ReportTime, beginTime);
            monitorInfoFilter &= monitorInfoFilterBuilder.Lt(x => x.ReportTime, endTime);

            var houlyDataList = await _reportRep.SearchAsync(monitorInfoFilter);

            var regionAndEnvList = houlyDataList.GroupBy(x => new { x.RegionCode, x.Environment }).Select(x => new { x.Key.RegionCode, x.Key.Environment });
            foreach (var item in regionAndEnvList)
            {
                var currentRegionDatas = houlyDataList.Where(x => x.RegionCode == item.RegionCode && x.Environment == item.Environment);
                if (!currentRegionDatas.Any())
                {
                    continue;
                }

                var resourceReport = new ResourceReportEntity()
                {
                    ResourceUuid = resourceUuid,
                    RegionCode = item.RegionCode,
                    Environment = item.Environment,
                    CycleType = ReportCycleType.Daily,
                    ReportTime = beginTime,
                    RequestQty = currentRegionDatas.Sum(x => x.RequestQty),
                    CreateTime = DateTime.Now
                };

                resourceReport.Duration = AnalysisDailyDuration(currentRegionDatas.Where(x => x.Duration != null).SelectMany(x => x.Duration));

                resourceReport.Spider = currentRegionDatas
                    .Where(x => x.Spider != null)
                    .SelectMany(x => x.Spider)
                    .GroupBy(x => x.Key)
                    .Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToDictionary(x => x.Key, y => y.Value);
                resourceReport.Browser = currentRegionDatas
                    .Where(x => x.Browser != null)
                    .SelectMany(x => x.Browser)
                    .GroupBy(x => x.Key)
                    .Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToDictionary(x => x.Key, y => y.Value);
                resourceReport.Os = currentRegionDatas
                    .Where(x => x.Os != null)
                    .SelectMany(x => x.Os)
                    .GroupBy(x => x.Key)
                    .Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToDictionary(x => x.Key, y => y.Value);
                resourceReport.StatusCode = currentRegionDatas
                    .Where(x => x.StatusCode != null)
                    .SelectMany(x => x.StatusCode)
                    .GroupBy(x => x.Key)
                    .Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToDictionary(x => x.Key, y => y.Value);

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

        private async Task HourlyReportStatisticAsync(string resourceUuid, DateTime beginTime, DateTime endTime)
        {
            var monitorInfoList = await ResourceMonitorInfoSearchCycleRangeRecords(resourceUuid, beginTime, endTime);
            var allSessionKey = monitorInfoList.Select(x => x.SessionKey).Distinct();
            var allSessions = await _sessionRep.SearchAsync(Builders<ResourceSessionEntity>.Filter.In(x => x.SessionKey, allSessionKey));
            var regionAndEnvList = allSessions.GroupBy(c => new { c.RegionCode, c.Environment }).Select(x => new { x.Key.RegionCode, x.Key.Environment });

            foreach (var item in regionAndEnvList)
            {
                var currentRegionSession = allSessions.Where(x => x.RegionCode == item.RegionCode && x.Environment == item.Environment).Select(x => x.SessionKey);
                var currentRegionMonitorInfos = monitorInfoList.Where(x => currentRegionSession.Contains(x.SessionKey)).SelectMany(x => x.Data);

                if (!currentRegionMonitorInfos.Any())
                {
                    continue;
                }

                var resourceReport = new ResourceReportEntity()
                {
                    ResourceUuid = resourceUuid,
                    RegionCode = item.RegionCode,
                    Environment = item.Environment,
                    CycleType = ReportCycleType.Hourly,
                    ReportTime = beginTime,
                    RequestQty = currentRegionMonitorInfos.LongCount(),
                    CreateTime = DateTime.Now
                };

                resourceReport.Duration = AnlysisHourlyDuration(currentRegionMonitorInfos);
                resourceReport.Spider = AnlysisHourlySpider(currentRegionMonitorInfos);
                resourceReport.Browser = AnlysisHourlyBrowser(currentRegionMonitorInfos);
                resourceReport.Os = AnlysisHourlyOperationSystem(currentRegionMonitorInfos);
                resourceReport.StatusCode = AnlysisHourlyStatusCode(currentRegionMonitorInfos);

                await InsertOrUpdateResourceReport(resourceReport);
            }
        }

        private async Task InsertOrUpdateResourceReport(ResourceReportEntity resourceReport)
        {
            var filter = Builders<ResourceReportEntity>.Filter.Where(x => x.ResourceUuid == resourceReport.ResourceUuid
            && x.RegionCode == resourceReport.RegionCode
            && x.Environment == resourceReport.Environment
            && x.CycleType == resourceReport.CycleType
            && x.ReportTime == resourceReport.ReportTime);
            var update = Builders<ResourceReportEntity>.Update
                .SetOnInsert(x => x.ResourceUuid, resourceReport.ResourceUuid)
                .SetOnInsert(x => x.RegionCode, resourceReport.RegionCode)
                .SetOnInsert(x => x.Environment, resourceReport.Environment)
                .SetOnInsert(x => x.CycleType, resourceReport.CycleType)
                .SetOnInsert(x => x.ReportTime, resourceReport.ReportTime)
                .Set(x => x.RequestQty, resourceReport.RequestQty)
                .Set(x => x.Duration, resourceReport.Duration)
                .Set(x => x.Spider, resourceReport.Spider)
                .Set(x => x.Browser, resourceReport.Browser)
                .Set(x => x.Os, resourceReport.Os)
                .Set(x => x.StatusCode, resourceReport.StatusCode)
                .Set(x => x.CreateTime, resourceReport.CreateTime);
            var result = await _reportRep.Collection.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });

        }

        /// <summary>
        /// Spider数据
        /// </summary>
        /// <param name="allData"></param>
        /// <returns></returns>
        private Dictionary<string, long> AnlysisHourlySpider(IEnumerable<MonitorInfoEntity> allData)
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
        private Dictionary<string, long> AnlysisHourlyBrowser(IEnumerable<MonitorInfoEntity> allData)
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
        private Dictionary<string, long> AnlysisHourlyOperationSystem(IEnumerable<MonitorInfoEntity> allData)
        {
            var osResult = allData.Where(x => x.Os != null)
                .Select(x => $"{x.Os.Family}-{x.Os.Major}").GroupBy(x => x).Select(x => KeyValuePair.Create(x.Key, x.LongCount()));
            if (!osResult.Any())
                return null;
            return new Dictionary<string, long>(osResult);
        }
        private Dictionary<string, long> AnlysisHourlyStatusCode(IEnumerable<MonitorInfoEntity> allData)
        {
            var spiderResult = allData.Select(x => x.StatusCode).GroupBy(x => x).Select(x => KeyValuePair.Create(x.Key.ToString(), x.LongCount()));
            if (!spiderResult.Any())
                return null;
            return new Dictionary<string, long>(spiderResult);
        }
        private Dictionary<string, long> AnlysisHourlyDuration(IEnumerable<MonitorInfoEntity> allData)
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

        private async Task<IList<ResourceMonitorInfoEntity>> ResourceMonitorInfoSearchCycleRangeRecords(string resourceUuid, DateTime beginTime, DateTime endTime)
        {
            var monitorInfoFilterBuilder = Builders<ResourceMonitorInfoEntity>.Filter;
            var monitorInfoFilter = monitorInfoFilterBuilder.Eq(x => x.ResourceUuid, resourceUuid);
            monitorInfoFilter &= monitorInfoFilterBuilder.Gte(x => x.Timestamp, beginTime);
            monitorInfoFilter &= monitorInfoFilterBuilder.Lt(x => x.Timestamp, endTime);
            var monitorInfos = await _monitorInfoRep.SearchAsync(monitorInfoFilter);
            return monitorInfos;
        }
    }
}
