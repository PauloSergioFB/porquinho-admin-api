using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Logs;

public class ApiLogResponseDto
{
    public string Id { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    public string Level { get; set; } = string.Empty;

    public string MessageTemplate { get; set; } = string.Empty;

    public string RenderedMessage { get; set; } = string.Empty;

    public string? UtcTimestamp { get; set; }

    public string? Exception { get; set; }

    public Dictionary<string, object?>? Properties { get; set; }

    public static ApiLogResponseDto FromEntity(ApiLog log)
    {
        return new ApiLogResponseDto
        {
            Id = log.Id,
            Timestamp = log.Timestamp,
            Level = log.Level,
            MessageTemplate = log.MessageTemplate,
            RenderedMessage = log.RenderedMessage,
            UtcTimestamp = log.UtcTimestamp,
            Exception = log.Exception,
            Properties = log.Properties?.ToDictionary()
        };
    }
}