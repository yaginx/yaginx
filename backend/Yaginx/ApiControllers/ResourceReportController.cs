using AgileLabs;
using Microsoft.AspNetCore.Mvc;
using Yaginx.DomainModels.MonitorModels;
using Yaginx.Services;

namespace Yaginx.ApiControllers;

[ApiController]
[Route("yaginx/api/resource/report")]
public class ResourceReportController : YaginxControllerBase
{
    private readonly IResourceReportRepository _resourceReportRepository;

    public ResourceReportController(IResourceReportRepository resourceReportRepository)
    {
        _resourceReportRepository = resourceReportRepository;
    }

    [HttpPost, Route("search")]
    public async Task<List<ResourceReportModel>> ReportSearch([FromBody] ReportSearchRequest request)
    {
        return await _resourceReportRepository.SearchAsync(request);
    }

    ///// <summary>
    ///// Report合并了Region
    ///// </summary>
    ///// <param name="request"></param>
    ///// <returns></returns>
    //[HttpPost, Route("search/combine_region")]
    //public async Task<List<ResourceReportModel>> ReportSearchCombineRegion([FromBody] ReportSearchRequest request)
    //{
    //    var filterBuilder = Builders<ResourceReportEntity>.Filter;
    //    var filter = filterBuilder.Eq(x => x.ResourceUuid, request.ResourceUuid);
    //    filter &= filterBuilder.Eq(x => x.CycleType, request.CycleType);
    //    filter &= filterBuilder.Gte(x => x.ReportTime, request.BeginTime.FromEpochSeconds().ToLocalTime());
    //    filter &= filterBuilder.Lt(x => x.ReportTime, request.EndTime.FromEpochSeconds().ToLocalTime());
    //    var resultList = await _reportRep.SearchAsync(filter);

    //    var combinedResult = resultList.GroupBy(x => new { x.ResourceUuid, x.CycleType, x.ReportTime }).Select(n => new ResourceReportModel
    //    {
    //        ResourceUuid = n.Key.ResourceUuid,
    //        CycleType = n.Key.CycleType,
    //        ReportTime = n.Key.ReportTime.GetEpochSeconds(),
    //        RequestQty = n.Sum(v => v.RequestQty),
    //        Duration = ResourceReportServcie.AnalysisDailyDuration(n.Where(x => x.Duration != null).SelectMany(m => m.Duration)).Select(x => KeyValuePair.Create(x.Key, x.Value)).ToList(),
    //        Spider = n.Where(x => x.Spider != null).SelectMany(m => m.Spider).GroupBy(x => x.Key).Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToList(),
    //        Browser = n.Where(x => x.Browser != null).SelectMany(x => x.Browser).GroupBy(x => x.Key).Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToList(),
    //        Os = n.Where(x => x.Os != null).SelectMany(x => x.Os).GroupBy(x => x.Key).Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToList(),
    //        StatusCode = n.Where(x => x.StatusCode != null).SelectMany(x => x.StatusCode).GroupBy(x => x.Key).Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToList()
    //    });

    //    return combinedResult.ToList();
    //}

    [HttpGet, Route("all_report_data")]
    public async Task<dynamic> HourlyReport(ReportCycleType cycleType, int period = 60)
    {
        if (!Enum.IsDefined<ReportCycleType>(cycleType))
        {
            throw new Exception($"无效的{nameof(cycleType)}, 可用的值{Enum.GetValues<ReportCycleType>().JoinStrings(",")}");
        }

        var requestData = new ReportSearchRequest
        {
            CycleType = cycleType
        };

        List<ResourceReportModel> combinedResult = await GetResult(cycleType, period, requestData);

        var xAxis = new List<long>();// combinedResult.Select(x => x.ReportTime).OrderBy(x => x).ToList();
        long step = 60;
        string timeFormat = "HH:mm";
        switch (cycleType)
        {
            case ReportCycleType.Minutely:
                step = 60;
                timeFormat = "HH:mm";
                break;
            case ReportCycleType.Hourly:
                step = 3600;
                timeFormat = "dd-HH";
                break;
            case ReportCycleType.Daily:
                step = 3600 * 24;
                timeFormat = "MM-dd";
                break;
            case ReportCycleType.Weekly:
                step = 3600 * 24 * 7;
                timeFormat = "MM-dd";
                break;
            default:
                break;
        }
        var dataList = new List<ReportItem>();

        for (var i = requestData.BeginTime; i <= requestData.EndTime; i += step)
        {
            var currentTime = i.FromEpochSeconds().ToLocalTime();
            var qty = combinedResult.FirstOrDefault(x => x.ReportTime == currentTime)?.RequestQty ?? 0;
            dataList.Add(new ReportItem(currentTime.ToString(timeFormat), qty) { TimeTs = i });
        }
        return dataList;
    }

    [HttpGet, Route("type_report_data")]
    public async Task<dynamic> DurationReport(ReportCycleType cycleType, string type, int period = 60)
    {
        if (!Enum.IsDefined<ReportCycleType>(cycleType))
        {
            throw new Exception($"无效的{nameof(cycleType)}, 可用的值{Enum.GetValues<ReportCycleType>().JoinStrings(",")}");
        }

        var requestData = new ReportSearchRequest
        {
            CycleType = cycleType
        };

        List<ResourceReportModel> combinedResult = await GetResult(cycleType, period, requestData);

        var xAxis = new List<long>();// combinedResult.Select(x => x.ReportTime).OrderBy(x => x).ToList();
        long step = 60;
        string timeFormat = "HH:mm";
        switch (cycleType)
        {
            case ReportCycleType.Minutely:
                step = 60;
                timeFormat = "HH:mm";
                break;
            case ReportCycleType.Hourly:
                step = 3600;
                timeFormat = "HH:mm";
                break;
            case ReportCycleType.Daily:
                step = 3600 * 24;
                timeFormat = "MM-dd";
                break;
            case ReportCycleType.Weekly:
                step = 3600 * 24 * 7;
                timeFormat = "MM-dd";
                break;
            default:
                break;
        }
        var dataList = new List<DurationReportItem>();
        var dataTypeProperty = typeof(ResourceReportModel).GetProperty(type);
        for (var i = requestData.BeginTime; i <= requestData.EndTime; i += step)
        {
            var currentTime = i.FromEpochSeconds().ToLocalTime();
            var typeDataItem = combinedResult.FirstOrDefault(x => x.ReportTime == currentTime);
            if (typeDataItem != null)
            {
                var typeDataList = dataTypeProperty.GetValue(typeDataItem, null) as List<KeyValuePair<string, long>>;
                if (typeDataList.Any())
                {
                    foreach (var item in typeDataList)
                    {
                        dataList.Add(new DurationReportItem { Time = currentTime.ToString(timeFormat), Value = item.Value, Type = item.Key });
                    }
                }
                else
                {
                    dataList.Add(new DurationReportItem { Time = currentTime.ToString(timeFormat), Value = 0, Type = "default" });
                }
            }
            else
            {
                dataList.Add(new DurationReportItem { Time = currentTime.ToString(timeFormat), Value = 0, Type = "default" });
            }
        }
        return dataList;
    }

    private async Task<List<ResourceReportModel>> GetResult(ReportCycleType cycleType, int period, ReportSearchRequest requestData)
    {
        if (period > 200)
            period = 200;

        var nowTime = DateTime.Now;

        switch (cycleType)
        {
            case ReportCycleType.Minutely:
                nowTime = nowTime.Date.AddHours(nowTime.Hour).AddMinutes(nowTime.Minute);
                requestData.BeginTime = nowTime.AddMinutes(-period).GetEpochSeconds();
                break;
            case ReportCycleType.Hourly:
                nowTime = nowTime.Date.AddHours(nowTime.Hour);
                requestData.BeginTime = nowTime.AddHours(-period).GetEpochSeconds();
                break;
            case ReportCycleType.Daily:
                nowTime = nowTime.Date;
                requestData.BeginTime = nowTime.AddDays(-period).GetEpochSeconds();
                break;
            case ReportCycleType.Weekly:
                nowTime = nowTime.Date;
                requestData.BeginTime = nowTime.AddDays(-period * 7).GetEpochSeconds();
                break;
            default:
                break;
        }
        requestData.EndTime = nowTime.GetEpochSeconds();

        var result = await _resourceReportRepository.SearchAsync(requestData);

        var combinedResult = result.GroupBy(x => new { x.ReportTime }).Select(n => new ResourceReportModel
        {
            ReportTime = n.Key.ReportTime,
            RequestQty = n.Sum(v => v.RequestQty),
            Duration = ResourceReportServcie.AnalysisDailyDuration(n.Where(x => x.Duration != null).SelectMany(m => m.Duration)).Select(x => KeyValuePair.Create(x.Key, x.Value)).ToList(),
            Spider = n.Where(x => x.Spider != null).SelectMany(m => m.Spider).GroupBy(x => x.Key).Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToList(),
            Browser = n.Where(x => x.Browser != null).SelectMany(x => x.Browser).GroupBy(x => x.Key).Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToList(),
            Os = n.Where(x => x.Os != null).SelectMany(x => x.Os).GroupBy(x => x.Key).Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToList(),
            StatusCode = n.Where(x => x.StatusCode != null).SelectMany(x => x.StatusCode).GroupBy(x => x.Key).Select(x => KeyValuePair.Create(x.Key, x.Sum(item => item.Value))).ToList()
        }).ToList();
        return combinedResult;
    }

    [HttpGet, Route("resource_report_data")]
    public async Task<dynamic> HourlyReport(string resourceUuid, ReportCycleType cycleType)
    {
        if (!Enum.IsDefined<ReportCycleType>(cycleType))
        {
            throw new Exception($"无效的{nameof(cycleType)}, 可用的值{Enum.GetValues<ReportCycleType>().JoinStrings(",")}");
        }

        var requestData = new ReportSearchRequest
        {
            ResourceUuid = resourceUuid,
            CycleType = cycleType
        };
        requestData.EndTime = DateTime.Now.GetEpochSeconds();
        requestData.BeginTime = DateTime.Now.AddDays(-90).GetEpochSeconds();

        var result = await _resourceReportRepository.SearchAsync(requestData);
        var xAxis = result.Select(x => x.ReportTime).Distinct().OrderBy(x => x).ToList();

        //var series = new List<Tuple<string, List<long>>>();

        //foreach (var item in legend)
        //{
        //    var data = result.Where(x => x.RegionCode == item);


        //    series.Add(Tuple.Create(item, dataList));
        //}
        //var chartOptions = new { xAxis = new { type = "" } };
        var dataList = new List<ReportItem>();
        foreach (var reportTime in xAxis)
        {
            var qty = result.FirstOrDefault(x => x.ReportTime == reportTime)?.RequestQty ?? 0;
            dataList.Add(new ReportItem(reportTime.ToLocalTime().ToString("HH:mm"), qty));
        }

        //ViewBag.Legend = JsonConvert.SerializeObject(legend);
        //ViewBag.xAxis = JsonConvert.SerializeObject(xAxis.Select(x => x.FromEpochSeconds().ToLocalTime().ToString("MM/dd HH(tt)")));
        //ViewBag.series = JsonConvert.SerializeObject(series);
        //return View();
        /*
          {
        "Date": "2010-01",
        "scales": 1998
    }
         */
        return dataList;
    }
    public class ReportItem
    {
        public ReportItem()
        {

        }

        public ReportItem(string time, long scales)
        {
            this.Time = time;
            this.Scales = scales;
        }
        public long TimeTs { get; set; }
        public string Time { get; set; }
        public long Scales { get; set; }
    }

    public class DurationReportItem
    {
        public string Time { get; set; }
        public long Value { get; set; }
        public string Type { get; set; }
    }
}
