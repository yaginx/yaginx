using AgileLabs.Storage.Mongo;
using MongoDB.Bson.Serialization.Attributes;

namespace Yaginx.DataStore.MongoStore.Entities
{
    /// <summary>
    /// 应用程序或者站点资源
    /// </summary>
    [MongoCollection(CollectionName = "Resource"), BsonIgnoreExtraElements(true)]
    public class ResourceEntity : MongoEntityBase
    {
        /// <summary>
        /// 字符串类型的标识
        /// </summary>
        [BsonElement(elementName: "ResId")]
        public string ResourceUuid { get; set; }

        /// <summary>
        /// 数据报告密钥
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string Name { get; set; }
    }
    public enum ReportCycleType
    {
        Hourly = 1,
        Daily = 2,
        Weekly = 3
    }
}
