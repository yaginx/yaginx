using AgileLabs.Storage.Mongo;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Yaginx.DataStore.MongoStore.Entities
{
    [MongoCollection(CollectionName = "ResourceReport"), BsonIgnoreExtraElements(true)]
    public class ResourceReportEntity : MongoEntityBase
    {
        [BsonElement(elementName: "ResId")]
        public string ResourceUuid { get; set; }
        public ReportCycleType CycleType { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ReportTime { get; set; }

        /// <summary>
        /// 总处理请求
        /// </summary>
        public long RequestQty { get; set; }
        public Dictionary<string, long> StatusCode { get; set; }
        public Dictionary<string, long> Spider { get; set; }
        public Dictionary<string, long> Browser { get; set; }
        public Dictionary<string, long> Os { get; set; }
        public Dictionary<string, long> Duration { get; set; }

        /// <summary>
        /// 数据统计时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTime { get; set; }
    }
}
