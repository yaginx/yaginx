using MongoDB.Bson;

namespace Yaginx.DataStore.MongoStore
{
    public interface IEntityBase
    {
        ObjectId Sysid { get; set; }
    }
}
