#nullable enable

#region

using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SharpStore.Entities;

#endregion

namespace SharpStore.Services
{
    public interface IMongoDb
    {
        void SetDatabase(string databaseName);
        void SetDatabase(IMongoDatabase database);
        IMongoDatabase GetDatabase(string databaseName);
        IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName);
        Task<TDocument> InsertDocumentAsync<TDocument>(TDocument document, string collectionName);
        Task<List<TDocument>> InsertDocumentsAsync<TDocument>(List<TDocument> documents, string collectionName);
        Task<List<TDocument>> GetAllDocumentsAsync<TDocument>(string collectionName);

        Task<ulong> CountDocuments<TDocument>(string collectionName, FilterDefinition<TDocument> filter);

        Task<TDocument?> GetDocumentAsync<TDocument>(FilterDefinition<TDocument> filter,
            string collectionName,
            ProjectionDefinition<TDocument>? projection = null);

        Task<List<TDocument>> GetDocumentsAsync<TDocument>(string collectionName,
            FilterDefinition<TDocument> filter,
            int skip = 0,
            int limit = int.MaxValue,
            SortDefinition<TDocument>? sort = null,
            ProjectionDefinition<TDocument>? projection = null);

        Task<bool> UpdateDocumentAsync<TDocument>(FilterDefinition<TDocument> filter,
            UpdateDefinition<TDocument> update, string collectionName);

        Task<bool> UpdateDocumentsAsync<TDocument>(FilterDefinition<TDocument> filter,
            UpdateDefinition<TDocument> update, string collectionName);

        Task<bool> DeleteDocumentAsync<TDocument>(FilterDefinition<TDocument> filter,
            string collectionName);

        Task<bool> DeleteDocumentsAsync<TDocument>(FilterDefinition<TDocument> filter,
            string collectionName);

        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// Code from MongoDd website.
        /// <para>Resource: https://mongodb.github.io/mongo-csharp-driver/2.12/getting_started/quick_tour/</para>
        /// <code>
        /// var models = new WriteModel&lt;BsonDocument&gt;[] 
        /// {
        ///     new InsertOneModel&lt;BsonDocument&gt;(new BsonDocument("_id", 4)),
        ///     new InsertOneModel&lt;BsonDocument&gt;(new BsonDocument("_id", 5)),
        ///     new InsertOneModel&lt;BsonDocument&gt;(new BsonDocument("_id", 6)),
        ///     new UpdateOneModel&lt;BsonDocument&gt;(
        ///         new BsonDocument("_id", 1), 
        ///         new BsonDocument("$set", new BsonDocument("x", 2))),
        ///     new DeleteOneModel&lt;BsonDocument&gt;(new BsonDocument("_id", 3)),
        ///     new ReplaceOneModel&lt;BsonDocument&gt;(
        ///         new BsonDocument("_id", 3), 
        ///         new BsonDocument("_id", 3).Add("x", 4))
        /// };
        /// </code>
        /// </example>
        /// <see href="https://mongodb.github.io/mongo-csharp-driver/2.12/getting_started/quick_tour/"/>
        /// <param name="writeModel"></param>
        /// <param name="writeOptions"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        Task<bool> BulkWriteAsync<TDocument>(List<WriteModel<TDocument>> writeModel,
            BulkWriteOptions writeOptions, string collectionName);

        Task<List<BsonDocument>> GetDatabasesAsync();
        Task<bool> DropDatabaseAsync(string databaseName);
        Task<bool> CreateCollectionAsync<TDocument>(string collectionName, CreateCollectionOptions<TDocument> options);
        Task<bool> DropCollectionAsync(string collectionName);

        /// <summary>
        /// Creating index for faster (if implemented correctly) queries.
        /// </summary>
        /// <example>
        /// Ascending and Descending indexing:
        /// <code>
        /// var keys = Builders&lt;BsonDocument&gt;.IndexKeys.Ascending("i");
        /// await collection.Indexes.CreateOneAsync(keys);
        /// </code>
        /// Text indexing:
        /// <code>
        /// var keys = Builders&lt;BsonDocument&gt;.IndexKeys.Text("content");
        /// await collection.Indexes.CreateOneAsync(keys);
        /// </code>
        /// </example>
        /// <param name="collectionName"></param>
        /// <param name="keysDefinition"></param>
        /// <returns></returns>
        Task<bool> CreateIndexAsync<TDocument>(string collectionName, CreateIndexModel<TDocument> keysDefinition);

        Task<bool> ReplaceDocument<TDocument>(FilterDefinition<TDocument> filter,
            TDocument document,
            string collectionName);
    }

    public class MongoDb : IMongoDb
    {
        private readonly MongoClient _client;
        private IMongoDatabase _database;

        public MongoDb(IDatabaseSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
            _database = _client.GetDatabase(settings.DatabaseName);
        }

        public void SetDatabase(string databaseName)
        {
            _database = _client.GetDatabase(databaseName);
        }


        public void SetDatabase(IMongoDatabase database)
        {
            _database = database;
        }

        #region Regular Tasks

        public IMongoDatabase GetDatabase(string databaseName)
        {
            return _client.GetDatabase(databaseName);
        }

        public IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName)
        {
            var collection = _database.GetCollection<TDocument>(collectionName);
            return collection;
        }

        public async Task<TDocument> InsertDocumentAsync<TDocument>(TDocument document, string collectionName)
        {
            await _database.GetCollection<TDocument>(collectionName).InsertOneAsync(document);
            return document;
        }

        public async Task<List<TDocument>> InsertDocumentsAsync<TDocument>(List<TDocument> documents,
            string collectionName)
        {
            await _database.GetCollection<TDocument>(collectionName).InsertManyAsync(documents);
            return documents;
        }

        public async Task<List<TDocument>> GetAllDocumentsAsync<TDocument>(string collectionName)
        {
            var documents = await GetCollection<TDocument>(collectionName)
                .Find(new BsonDocument()).ToListAsync();
            return documents;
        }

        public async Task<ulong> CountDocuments<TDocument>(string collectionName, FilterDefinition<TDocument> filter)
        {
            var collection = GetCollection<TDocument>(collectionName);
            var count = await collection.CountDocumentsAsync(filter);
            return (ulong)count;
        }


        public async Task<TDocument?> GetDocumentAsync<TDocument>(FilterDefinition<TDocument> filter,
            string collectionName,
            ProjectionDefinition<TDocument>? projection = null)
        {
            var collection = GetCollection<TDocument>(collectionName);
            var document = collection.Find(filter);
            if (projection is not null)
                document.Project(projection);
            var finalDocument = await document.SingleOrDefaultAsync();
            // Generic methods can NOT return null instead, you need to use default.
            return finalDocument;
        }

        public async Task<List<TDocument>> GetDocumentsAsync<TDocument>(string collectionName,
            FilterDefinition<TDocument> filter,
            int skip = 0,
            int limit = int.MaxValue,
            SortDefinition<TDocument>? sort = null,
            ProjectionDefinition<TDocument>? projection = null)
        {
            var collection = GetCollection<TDocument>(collectionName);
            var documents = collection.Find(filter)
                .Skip(skip)
                .Limit(limit);
            if (sort is not null)
                documents.Sort(sort);
            if (projection is not null)
                documents.Project<TDocument>(projection);
            return await documents.ToListAsync();
        }

        public async Task<bool> UpdateDocumentAsync<TDocument>(FilterDefinition<TDocument> filter,
            UpdateDefinition<TDocument> update, string collectionName)
        {
            var collection = GetCollection<TDocument>(collectionName);
            var updateResult = await collection.UpdateOneAsync(filter, update);
            if (updateResult.ModifiedCount > 0)
                return true;
            return false;
        }

        public async Task<bool> UpdateDocumentsAsync<TDocument>(FilterDefinition<TDocument> filter,
            UpdateDefinition<TDocument> update, string collectionName)
        {
            var collection = GetCollection<TDocument>(collectionName);
            var updateResult = await collection.UpdateManyAsync(filter, update);
            if (updateResult.ModifiedCount > 0)
                return true;
            return false;
        }

        public async Task<bool> DeleteDocumentAsync<TDocument>(FilterDefinition<TDocument> filter,
            string collectionName)
        {
            var collection = GetCollection<TDocument>(collectionName);
            var deleteResult = await collection.DeleteOneAsync(filter);
            if (deleteResult.DeletedCount > 0)
                return true;
            return false;
        }

        public async Task<bool> DeleteDocumentsAsync<TDocument>(FilterDefinition<TDocument> filter,
            string collectionName)
        {
            var collection = GetCollection<TDocument>(collectionName);
            var deleteResult = await collection.DeleteManyAsync(filter);
            if (deleteResult.DeletedCount > 0)
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// Code from MongoDd website.
        /// <para>Resource: https://mongodb.github.io/mongo-csharp-driver/2.12/getting_started/quick_tour/</para>
        /// <code>
        /// var models = new WriteModel&lt;BsonDocument&gt;[] 
        /// {
        ///     new InsertOneModel&lt;BsonDocument&gt;(new BsonDocument("_id", 4)),
        ///     new InsertOneModel&lt;BsonDocument&gt;(new BsonDocument("_id", 5)),
        ///     new InsertOneModel&lt;BsonDocument&gt;(new BsonDocument("_id", 6)),
        ///     new UpdateOneModel&lt;BsonDocument&gt;(
        ///         new BsonDocument("_id", 1), 
        ///         new BsonDocument("$set", new BsonDocument("x", 2))),
        ///     new DeleteOneModel&lt;BsonDocument&gt;(new BsonDocument("_id", 3)),
        ///     new ReplaceOneModel&lt;BsonDocument&gt;(
        ///         new BsonDocument("_id", 3), 
        ///         new BsonDocument("_id", 3).Add("x", 4))
        /// };
        /// </code>
        /// </example>
        /// <see href="https://mongodb.github.io/mongo-csharp-driver/2.12/getting_started/quick_tour/"/>
        /// <param name="writeModel"></param>
        /// <param name="writeOptions"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public async Task<bool> BulkWriteAsync<TDocument>(List<WriteModel<TDocument>> writeModel,
            BulkWriteOptions writeOptions, string collectionName)
        {
            var collection = GetCollection<TDocument>(collectionName);
            var response = await collection.BulkWriteAsync(writeModel, writeOptions);
            // TODO: Do something with these (make promise response maybe ?)
            // var deletedCount = response.DeletedCount;
            // var insertedCount = response.InsertedCount;
            // var modifiedCount = response.ModifiedCount;
            if (response.MatchedCount > 0)
                return true;
            return false;
        }

        #endregion

        #region Administrative Tasks

        public async Task<List<BsonDocument>> GetDatabasesAsync()
        {
            var databases = (await _client.ListDatabasesAsync()).ToList();
            return databases;
        }

        public async Task<bool> DropDatabaseAsync(string databaseName)
        {
            await _client.DropDatabaseAsync(databaseName);
            return true;
        }

        public async Task<bool> CreateCollectionAsync<TDocument>(string collectionName,
            CreateCollectionOptions<TDocument> options)
        {
            await _database.CreateCollectionAsync(collectionName, options);
            return true;
        }

        public async Task<bool> DropCollectionAsync(string collectionName)
        {
            await _database.DropCollectionAsync(collectionName);
            return true;
        }

        /// <summary>
        /// Creating index for faster (if implemented correctly) queries.
        /// </summary>
        /// <example>
        /// Ascending and Descending indexing:
        /// <code>
        /// var keys = Builders&lt;BsonDocument&gt;.IndexKeys.Ascending("i");
        /// await collection.Indexes.CreateOneAsync(keys);
        /// </code>
        /// Text indexing:
        /// <code>
        /// var keys = Builders&lt;BsonDocument&gt;.IndexKeys.Text("content");
        /// await collection.Indexes.CreateOneAsync(keys);
        /// </code>
        /// </example>
        /// <param name="collectionName"></param>
        /// <param name="keysDefinition"></param>
        /// <returns></returns>
        public async Task<bool> CreateIndexAsync<TDocument>(string collectionName,
            CreateIndexModel<TDocument> keysDefinition)
        {
            await GetCollection<TDocument>(collectionName).Indexes.CreateOneAsync(keysDefinition);
            return true;
        }

        public async Task<bool> ReplaceDocument<TDocument>(FilterDefinition<TDocument> filter,
            TDocument document, string collectionName)
        {
            var collection = GetCollection<TDocument>(collectionName);
            var response = await collection.ReplaceOneAsync(filter, document);
            if (response.MatchedCount > 0)
                return true;
            return false;
        }

        #endregion
    }
}