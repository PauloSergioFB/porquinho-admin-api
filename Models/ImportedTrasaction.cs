using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace PorquinhoApi.Models;

[BsonIgnoreExtraElements]
public class ImportedTransaction
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    public int OracleTransactionId { get; set; }

    public decimal TransactionValue { get; set; }

    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; }

    public bool HasOccurred { get; set; }

    public bool IsAutoConfirmed { get; set; }

    public string? Observation { get; set; }

    public DateTime OracleCreatedAt { get; set; }

    public DateTime? OracleUpdatedAt { get; set; }

    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}