using MongoDB.Bson;
using MongoDB.Driver;

namespace Domain.Repositories;

public interface IRecoverRepository
{
    Task<BsonDocument> InsertDocumentAsync(string collectionName, BsonDocument document, CancellationToken cts);
    Task<BsonDocument> UpdateDocumentAsync<TValue>(
        string collectionName, 
        string idFieldName, 
        TValue idValue, 
        string fieldNameToUpdate, 
        BsonValue newValue, 
        CancellationToken cts
    );
    bool Any(string collectionName, FilterDefinition<BsonDocument> filter);
    Task<BsonDocument> FindDocumentAsync<T>(string collectionName, string fieldName, T value);
    Task DeleteDocumentAsync(string collectionName, string fieldName, string value, CancellationToken cts);
}