using AgileLabs.Storage.Mongo;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Yaginx.DataStore.MongoStore;

namespace Yaginx.DataStore.MongoStore.Entities
{
    /// <summary>
    /// 具体的提交的原始数据
    /// </summary>
    [MongoCollection(CollectionName = "MonitorInfo"), BsonIgnoreExtraElements(true)]
    public class ResourceMonitorInfoEntity : MongoEntityBase
    {
        [BsonElement(elementName: "res_id")]
        public string ResourceUuid { get; set; }
        [BsonElement(elementName: "ts")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; }
        [BsonElement(elementName: "data")]
        public List<MonitorInfoEntity> Data { get; set; }
    }
    public class MonitorInfoEntity
    {
        [BsonIgnoreIfNull]
        public UserAgentInfo Ua { get; set; }
        [BsonIgnoreIfNull]
        public DeviceInfo Device { get; set; }
        [BsonIgnoreIfNull]
        public OsInfo Os { get; set; }

        [DataMember(Name = "h")]
        [BsonElement(elementName: "h"), BsonIgnoreIfNull]
        public string Host { get; set; }
        [DataMember(Name = "p")]
        [BsonElement(elementName: "p"), BsonIgnoreIfNull]
        public string Path { get; set; }
        [DataMember(Name = "qs")]
        [BsonElement(elementName: "qs"), BsonIgnoreIfNull]
        public string QueryString { get; set; }
        [DataMember(Name = "r")]
        [BsonElement(elementName: "r"), BsonIgnoreIfNull]
        public string Referer { get; set; }
        [DataMember(Name = "ua")]
        [BsonElement(elementName: "ua"), BsonIgnoreIfNull]
        public string UserAgent { get; set; }
        [DataMember(Name = "ip")]
        [BsonElement(elementName: "ip"), BsonIgnoreIfNull]
        public string Ip { get; set; }
        [DataMember(Name = "l")]
        [BsonElement(elementName: "l"), BsonIgnoreIfNull]
        public string Lang { get; set; }
        [DataMember(Name = "sc")]
        [BsonElement(elementName: "sc"), BsonIgnoreIfNull]
        public int StatusCode { get; set; }
        [DataMember(Name = "d")]
        [BsonElement(elementName: "d"), BsonIgnoreIfNull]
        public long Duration { get; set; }
    }
    public class UserAgentInfo
    {
        [BsonElement(elementName: "fm"), BsonIgnoreIfNull] public string Family { get; set; }
        [BsonElement(elementName: "mj"), BsonIgnoreIfNull] public string Major { get; set; }
    }

    public class DeviceInfo
    {
        //
        // 摘要:
        //     Returns true if the device is likely to be a spider or a bot device
        [BsonElement(elementName: "is_spid"), BsonIgnoreIfNull] public bool IsSpider { get; set; }
        //
        // 摘要:
        //     The brand of the device
        [BsonElement(elementName: "brd"), BsonIgnoreIfNull] public string Brand { get; set; }
        //
        // 摘要:
        //     The family of the device, if available
        [BsonElement(elementName: "fm"), BsonIgnoreIfNull] public string Family { get; set; }
        //
        // 摘要:
        //     The model of the device, if available
        [BsonElement(elementName: "md"), BsonIgnoreIfNull] public string Model { get; set; }
    }

    public class OsInfo
    {
        //
        // 摘要:
        //     The familiy of the OS
        [BsonElement(elementName: "fm"), BsonIgnoreIfNull] public string Family { get; set; }
        //
        // 摘要:
        //     The major version of the OS, if available
        [BsonElement(elementName: "mj"), BsonIgnoreIfNull] public string Major { get; set; }
    }
}
