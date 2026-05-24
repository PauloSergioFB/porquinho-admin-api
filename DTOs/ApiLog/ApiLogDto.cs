namespace PorquinhoApi.DTOs.Logs;

public class CreateApiLogDto
{
    public string Level { get; set; } = string.Empty;

    public string MessageTemplate { get; set; } = string.Empty;

    public string RenderedMessage { get; set; } = string.Empty;

    public string? Exception { get; set; }
}
