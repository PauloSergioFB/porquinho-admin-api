using MongoDB.Bson;
using MongoDB.Driver;
using PorquinhoApi.Models;

namespace PorquinhoApi.Services;

public class ImportedTransactionService
{
    private readonly IMongoCollection<ImportedTransaction> _transactions;

    public ImportedTransactionService(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDb:ConnectionString"];
        var databaseName = configuration["MongoDb:DatabaseName"] ?? "porquinho";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        _transactions = database.GetCollection<ImportedTransaction>("imported_transactions");
    }

    public async Task<List<ImportedTransaction>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;

        return await _transactions
            .Find(Builders<ImportedTransaction>.Filter.Empty)
            .SortByDescending(t => t.TransactionDate)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<ImportedTransaction?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        return await _transactions
            .Find(t => t.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<ImportedTransaction> CreateAsync(ImportedTransaction transaction)
    {
        await _transactions.InsertOneAsync(transaction);
        return transaction;
    }

    public async Task<bool> UpdateAsync(string id, ImportedTransaction transaction)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        transaction.Id = id;
        transaction.UpdatedAt = DateTime.UtcNow;

        var result = await _transactions.ReplaceOneAsync(t => t.Id == id, transaction);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        var result = await _transactions.DeleteOneAsync(t => t.Id == id);

        return result.DeletedCount > 0;
    }

    public async Task<long> DeleteAllAsync()
    {
        var result = await _transactions.DeleteManyAsync(Builders<ImportedTransaction>.Filter.Empty);
        return result.DeletedCount;
    }

    public async Task<int> ImportFromOracleAsync(
        List<Transaction> oracleTransactions)
    {
        var importedTransactions =
            oracleTransactions.Select(t =>
                new ImportedTransaction
                {
                    OracleTransactionId = t.TransactionId,
                    TransactionValue = t.TransactionValue,
                    Description = t.Description,
                    TransactionDate = t.TransactionDate,
                    HasOccurred = t.HasOccurred,
                    IsAutoConfirmed = t.IsAutoConfirmed,
                    Observation = t.Observation,
                    OracleCreatedAt = t.CreatedAt,
                    OracleUpdatedAt = t.UpdatedAt,
                    ImportedAt = DateTime.UtcNow
                }
            ).ToList();

        if (importedTransactions.Count > 0)
        {
            await _transactions.InsertManyAsync(
                importedTransactions
            );
        }

        return importedTransactions.Count;
    }
}