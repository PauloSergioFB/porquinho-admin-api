using Microsoft.AspNetCore.Http.HttpResults;
using PorquinhoApi.Models;
using PorquinhoApi.Services;

using PorquinhoApi.DTOs.Logs;

public static class ApiLogsEndpoints
{
    public static void MapApiLogEndpoints(
        this WebApplication app)
    {
        var logs = app.MapGroup("/logs")
            .WithTags("Logs")
            .RequireAuthorization();

        logs.MapGet("/", GetAllLogs)
            .WithName("GetAllLogs")
            .WithSummary("Retorna todos os logs")
            .WithDescription("""
                Obtém os logs armazenados no MongoDB.
                """)
            .Produces<Ok<List<ApiLogResponseDto>>>(
                StatusCodes.Status200OK
                );

        logs.MapGet("/{id}", GetLogById)
            .WithName("GetLogById")
            .WithSummary("Retorna um log específico")
            .WithDescription("""
                Obtém um log pelo ID informado.
                """)
            .Produces<Ok<ApiLogResponseDto>>(
                StatusCodes.Status200OK
                )
            .Produces<NotFound>(
                StatusCodes.Status404NotFound
            );

        logs.MapPost("/", CreateLog)
            .WithName("CreateLog")
            .WithSummary("Cria um log")
            .WithDescription("""
                Cria manualmente um log no MongoDB.
                """)
            .Produces<Created<ApiLogResponseDto>>(
                StatusCodes.Status201Created
            );

        logs.MapPut("/{id}", UpdateLog)
            .WithName("UpdateLog")
            .WithSummary("Atualiza um log")
            .WithDescription("""
                Atualiza um log armazenado no MongoDB.
                """)
            .Produces<Ok<ApiLogResponseDto>>(
                StatusCodes.Status200OK
            )
            .Produces<NotFound>(
                StatusCodes.Status404NotFound
            );

        logs.MapDelete("/{id}", DeleteLogById)
            .WithName("DeleteLogById")
            .WithSummary("Remove um log")
            .WithDescription("""
                Remove um log do MongoDB pelo ID informado.
                """)
            .Produces<NoContent>(
                StatusCodes.Status204NoContent
            )
            .Produces<NotFound>(
                StatusCodes.Status404NotFound
            );

        logs.MapDelete("/", DeleteAllLogs)
            .WithName("DeleteAllLogs")
            .WithSummary("Remove todos os logs")
            .WithDescription("""
                Remove todos os logs armazenados no MongoDB.
                """)
            .Produces<Ok<object>>(
                StatusCodes.Status200OK
            );
    }

    static async Task<Ok<List<ApiLogResponseDto>>> GetAllLogs(
        ApiLogService service,
        int page = 1,
        int pageSize = 20)
    {
        var logs = await service.GetAllAsync(page, pageSize);

        var response = logs
            .Select(ApiLogResponseDto.FromEntity)
            .ToList();

        return TypedResults.Ok(response);
    }

    static async Task<Results<Ok<ApiLogResponseDto>, NotFound>> GetLogById(
        string id,
        ApiLogService service)
    {
        var log = await service.GetByIdAsync(id);

        if (log is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(ApiLogResponseDto.FromEntity(log));
    }

    static async Task<Created<ApiLogResponseDto>> CreateLog(
        CreateApiLogDto dto,
        ApiLogService service)
    {
        var log = new ApiLog
        {
            Timestamp = DateTime.UtcNow,
            UtcTimestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ssZ"),
            Level = dto.Level,
            MessageTemplate = dto.MessageTemplate,
            RenderedMessage = dto.RenderedMessage,
            Exception = dto.Exception
        };

        await service.CreateAsync(log);

        return TypedResults.Created(
            $"/logs/{log.Id}",
            ApiLogResponseDto.FromEntity(log)
        );
    }

    static async Task<Results<Ok<ApiLogResponseDto>, NotFound>> UpdateLog(
        string id,
        UpdateApiLogDto dto,
        ApiLogService service)
    {
        var existingLog = await service.GetByIdAsync(id);

        if (existingLog is null)
            return TypedResults.NotFound();

        existingLog.Level = dto.Level;
        existingLog.MessageTemplate = dto.MessageTemplate;
        existingLog.RenderedMessage = dto.RenderedMessage;
        existingLog.Exception = dto.Exception;

        await service.UpdateAsync(id, existingLog);

        return TypedResults.Ok(
            ApiLogResponseDto.FromEntity(existingLog)
        );
    }

    static async Task<
        Results<NoContent, NotFound>
    > DeleteLogById(
        string id,
        ApiLogService service)
    {
        var deleted =
            await service.DeleteByIdAsync(id);

        if (!deleted)
            return TypedResults.NotFound();

        return TypedResults.NoContent();
    }

    static async Task<IResult> DeleteAllLogs(
        ApiLogService service)
    {
        var deletedCount = await service.DeleteAllAsync();

        return TypedResults.Ok(new
        {
            deletedCount
        });
    }
}