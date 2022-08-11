using MongoDB.Driver;
using SharpStore.Entities;
using SharpStore.Services;

namespace SharpStore.Repositories;

public interface IStoreRepository
{
    Task<Store> CreateStore(Store store);
    Task<List<Store>> GetStores();
    Task<List<Store>> GetStoreByUser(string userId);
    Task<Store> UpdateStore(Store store);
}

public class StoreRepository : IStoreRepository
{
    private readonly IMongoDb _db;
    private const string CollectionName = "Store";

    public StoreRepository(IMongoDb db)
    {
        _db = db;
    }
    public async Task<Store> CreateStore(Store store)
    {
        var result = await _db.InsertDocumentAsync(store, CollectionName);
        return result;
    }

    public async Task<List<Store>> GetStores()
    {
        var result = await _db.GetDocumentsAsync(CollectionName, FilterDefinition<Store>.Empty);
        return result;
    }

    public async Task<List<Store>> GetStoreByUser(string userId)
    {
        var filter = Builders<Store>.Filter.Where(store => store.OwnerId == userId);
        var result = await _db.GetDocumentsAsync(CollectionName, filter);
        return result;
    }

    public async Task<Store> UpdateStore(Store store)
    {
        var filter = Builders<Store>.Filter.Where(st => st.StoreId == store.StoreId);
        var result = await _db.ReplaceDocument(filter, store, CollectionName);
        if (result)
        {
            return store;
        }

        return null;
    }
}