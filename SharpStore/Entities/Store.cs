using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharpStore.Entities;

public class Store
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string StoreId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string OwnerId { get; set; }
    public string AvatarId { get; set; }
}