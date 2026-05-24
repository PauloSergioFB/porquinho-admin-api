using MongoDB.Bson;

namespace PorquinhoApi.Utils;

public static class BsonExtensions
{
    public static Dictionary<string, object?> ToDictionary(this BsonDocument document)
    {
        return document.Elements.ToDictionary(
            element => element.Name,
            element => ConvertBsonValue(element.Value)
        );
    }

    private static object? ConvertBsonValue(BsonValue value)
    {
        if (value.IsBsonNull)
            return null;

        return value.BsonType switch
        {
            BsonType.String => value.AsString,
            BsonType.Int32 => value.AsInt32,
            BsonType.Int64 => value.AsInt64,
            BsonType.Double => value.AsDouble,
            BsonType.Boolean => value.AsBoolean,
            BsonType.DateTime => value.ToUniversalTime(),
            BsonType.Document => value.AsBsonDocument.ToDictionary(),
            BsonType.Array => value.AsBsonArray
                .Select(ConvertBsonValue)
                .ToList(),
            _ => value.ToString()
        };
    }
}