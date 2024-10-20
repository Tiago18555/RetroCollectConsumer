using Domain.Repositories;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class MongoRepository: IRecoverRepository
{
    private readonly MongoDBContext _context;

    public MongoRepository(MongoDBContext context)
    {
        _context = context;
    }

    public async Task<BsonDocument> InsertDocumentAsync(string collectionName, BsonDocument document, CancellationToken cts)
    {
        var collection = _context.GetCollection<BsonDocument>(collectionName);
        await collection.InsertOneAsync(document, null, cts);
        return document;
    }

    public async Task<BsonDocument> UpdateDocumentAsync<TValue>(
        string collectionName, 
        string idFieldName, 
        TValue idValue, 
        string fieldNameToUpdate, 
        BsonValue newValue, 
        CancellationToken cts
    )
    {
        var filter = Builders<BsonDocument>.Filter.Eq(idFieldName, BsonValue.Create(idValue));
        var update = Builders<BsonDocument>.Update.Set(fieldNameToUpdate, newValue);
        var collection = _context.GetCollection<BsonDocument>(collectionName);
        return await collection.FindOneAndUpdateAsync(filter, update, null, cts);            
    }

    public bool Any(string collectionName, FilterDefinition<BsonDocument> filter)
    {
        var collection = _context.GetCollection<BsonDocument>(collectionName);
        var result = collection.Find(filter).Any();
        return result;
    }
    
    public async Task<BsonDocument> FindDocumentAsync<T>(string collectionName, string fieldName, T value)
    {
        var filter = Builders<BsonDocument>.Filter.Eq(fieldName, value);
        var collection = _context.GetCollection<BsonDocument>(collectionName);
        var res = await collection.FindAsync(filter);
        return await res.FirstOrDefaultAsync();
    }

    public async Task DeleteDocumentAsync(string collectionName, string fieldName, string value, CancellationToken cts)
    {
        var filter = Builders<BsonDocument>.Filter.Eq(fieldName, value);
        var collection = _context.GetCollection<BsonDocument>(collectionName);
        await collection.DeleteOneAsync(filter, null, cts);
    }
}
