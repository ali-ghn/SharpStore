using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharpStore.Entities;

public class User : IdentityUser
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AvatarId { get; set; }
}