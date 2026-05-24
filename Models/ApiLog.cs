using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace PorquinhoApi.Models;

[BsonIgnoreExtraElements]
public class ApiLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    public string Level { get; set; } = string.Empty;

    public string MessageTemplate { get; set; } = string.Empty;

    public string RenderedMessage { get; set; } = string.Empty;

    public BsonDocument? Properties { get; set; }

    public string? UtcTimestamp { get; set; }

    public string? Exception { get; set; }
}