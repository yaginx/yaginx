using AgileLabs.Storage.Mongo;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Yaginx.DataStore.MongoStore
{
    [BsonIgnoreExtraElements(true), Serializable]
    public class MongoEntityBase : MongoEntity, IEntityBase, IMongoEntity<ObjectId>
    {
        public MongoEntityBase()
        {

        }
    }
}
