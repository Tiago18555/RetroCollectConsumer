using MongoDB.Bson;
using MongoDB.Driver;

namespace Domain.Repositories;

public interface IRecoverRepository
{
    Task<BsonDocument> InsertDocumentAsync(string collectionName, BsonDocument document);
    Task<BsonDocument> UpdateDocumentAsync<TValue>(string collectionName, string idFieldName, TValue idValue, string fieldNameToUpdate, BsonValue newValue);
    int CountFailedAttemptsSinceLastSuccess(Guid userId);
    bool Any(string collectionName, FilterDefinition<BsonDocument> filter);
    Task<BsonDocument> FindDocumentAsync<T>(string collectionName, string fieldName, T value);
    void DeleteDocument(string collectionName, string fieldName, string value);
}