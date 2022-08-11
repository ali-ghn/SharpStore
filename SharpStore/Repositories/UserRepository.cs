using System.Collections.Immutable;
using MongoDB.Driver;
using SharpStore.Entities;
using SharpStore.Services;

namespace SharpStore.Repositories;

public interface IUserRepository
{
    Task<User> GetUserByEmail(string email);
    Task<User> GetUserById(string id);
}

public class UserRepository : IUserRepository
{
    private readonly IMongoDb _db;
    private const string CollectionName = "User";

    public UserRepository(IMongoDb db)
    {
        _db = db;
    }
    
    public async Task<User> GetUserByEmail(string email)
    {
        var filter = Builders<User>.Filter.Where(user => user.Email == email);
        var result = await _db.GetDocumentAsync(filter, CollectionName);
        return result;
    }

    public async Task<User> GetUserById(string id)
    {
        var filter = Builders<User>.Filter.Where(user => user.UserId == id);
        var result = await _db.GetDocumentAsync(filter, CollectionName);
        return result;
    }
}