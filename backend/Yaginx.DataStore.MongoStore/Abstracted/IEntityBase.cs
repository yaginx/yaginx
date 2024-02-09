using MongoDB.Bson;

namespace Yaginx.DataStore.MongoStore.Abstracted
{
    public interface IEntityBase
    {
        ObjectId Sysid { get; set; }
    }
}
