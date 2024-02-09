using Microsoft.AspNetCore.Http;

namespace Yaginx.Infrastructure;

internal static class InternalExtensions
{
    public static long GetEpochMilliseconds(this DateTime date)
    {
        TimeSpan t = TimeZoneInfo.ConvertTimeToUtc(date) - new DateTime(1970, 1, 1);
        return (long)t.TotalMilliseconds;
    }
    public static bool IsNullOrWhitespace(this string obj)
    {
        return string.IsNullOrWhiteSpace(obj);
    }

    public static T GetHeaderValueAs<T>(this IHeaderDictionary header, string headerName)
    {
        if (header?.TryGetValue(headerName, out var values) ?? false)
        {
            string rawValues = values.ToString();   // writes out as Csv when there are multiple.

            if (!string.IsNullOrEmpty(rawValues))
            {
                return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
        }
        return default;
    }
    public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
    {
        if (string.IsNullOrWhiteSpace(csvList))
            return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

        return csvList
            .TrimEnd(',')
            .Split(',')
            .AsEnumerable()
            .Select(s => s.Trim())
            .ToList();
    }
    public static string GetClientIp(this HttpContext httpContext, bool tryUseXForwardHeader = true)
    {
        var header = httpContext.Request.Headers;
        string ip = string.Empty;

        // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
        // for 99% of cases however it has been suggested that a better (although tedious)
        // approach might be to read each IP from right to left and use the first public IP.
        // http://stackoverflow.com/a/43554000/538763
        //
        if (tryUseXForwardHeader)
        {
            ip = header.GetHeaderValueAs<string>("X-Original-Forwarded-For").SplitCsv().FirstOrDefault();

            if (ip.IsNullOrWhitespace())
            {
                ip = header.GetHeaderValueAs<string>("X-Forwarded-For").SplitCsv().FirstOrDefault();
            }
        }

        // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
        if (ip.IsNullOrWhitespace() && httpContext?.Connection?.RemoteIpAddress != null)
        {
            ip = httpContext.Connection.RemoteIpAddress.ToString();
        }

        if (ip.IsNullOrWhitespace())
        {
            ip = header.GetHeaderValueAs<string>("REMOTE_ADDR");
        }

        return ip;
    }
}