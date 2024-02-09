using AgileLabs.Storage.Mongo;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Text;

namespace Yaginx.DataStore.MongoStore.Entities
{
    [MongoCollection(CollectionName = "ResourceSession"), BsonIgnoreExtraElements(true)]
    public class ResourceSessionEntity : MongoEntityBase
    {
        [BsonElement(elementName: "ResId")]
        public string ResourceUuid { get; set; }
        [BsonElement(elementName: "ResName")]
        public string ResourceName { get; set; }
        public string SessionKey { get; set; }
        public string ServerName { get; set; }
        public string AppName { get; set; }
        public string RegionCode { get; set; }
        [BsonElement(elementName: "Env")]
        public string Environment { get; set; }
        public string Ip { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最近一次心跳包时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime LastBeatTime { get; set; }
    }
}
