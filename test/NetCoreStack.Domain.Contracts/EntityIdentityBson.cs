using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NetCoreStack.Contracts;

namespace NetCoreStack.Domain.Contracts
{
    public class EntityIdentityBson : IEntity, IEntityIdentity<string>
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonIgnore]
        public ObjectState ObjectState { get; set; }
    }
}
