using MongoDB.Bson;
using MongoDB.Driver;
using PorquinhoApi.Models;

namespace PorquinhoApi.Services;

public class ApiLogService
{
    private readonly IMongoCollection<ApiLog> _logs;

    public ApiLogService(IConfiguration configuration)
    {
        var connectionString =
            configuration["MongoDb:ConnectionString"];

        var databaseName =
            configuration["MongoDb:DatabaseName"];

        var client = new MongoClient(connectionString);

        var database = client.GetDatabase(databaseName);

        _logs = database.GetCollection<ApiLog>("api_logs");
    }

    public async Task<List<ApiLog>> GetAllAsync(
        int page = 1,
        int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;

        return await _logs
            .Find(Builders<ApiLog>.Filter.Empty)
            .SortByDescending(log => log.Timestamp)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<ApiLog?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        return await _logs
            .Find(log => log.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<ApiLog> CreateAsync(ApiLog log)
    {
        await _logs.InsertOneAsync(log);

        return log;
    }

    public async Task<bool> UpdateAsync(
        string id,
        ApiLog updatedLog)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        updatedLog.Id = id;

        var result = await _logs.ReplaceOneAsync(
            log => log.Id == id,
            updatedLog
        );

        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        var result = await _logs
            .DeleteOneAsync(log => log.Id == id);

        return result.DeletedCount > 0;
    }

    public async Task<long> DeleteAllAsync()
    {
        var result = await _logs
            .DeleteManyAsync(
                Builders<ApiLog>.Filter.Empty
            );

        return result.DeletedCount;
    }
}