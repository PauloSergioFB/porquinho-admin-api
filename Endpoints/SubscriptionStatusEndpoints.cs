using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PorquinhoApi.Data;
using PorquinhoApi.DTOs.SubscriptionStatuses;
using PorquinhoApi.Filters;
using PorquinhoApi.Models;
using PorquinhoApi.Services;
using PorquinhoApi.Utils;

namespace PorquinhoApi.Endpoints;

public static class SubscriptionStatusesEndpoints
{
    public static void MapSubscriptionStatusEndpoints(this WebApplication app)
    {
        var subscriptionStatuses = app.MapGroup("/subscription-statuses").WithTags("SubscriptionStatuses");

        subscriptionStatuses.MapGet("/", GetAllSubscriptionStatuses)
            .WithSummary("Retorna todos os status de subscrição cadastrados")
            .WithDescription("Obtém uma lista completa de status de subscrição do sistema")
            .Produces<Ok<PagedResponse<SubscriptionStatusResponseDto>>>(StatusCodes.Status200OK);

        subscriptionStatuses.MapGet("/{id:int}", GetSubscriptionStatusById)
            .WithSummary("Retorna um status de subscrição específico")
            .WithDescription("Obtém um status de subscrição pelo ID informado")
            .Produces<Ok<SubscriptionStatusResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptionStatuses.MapPost("/", CreateSubscriptionStatus)
            .WithSummary("Cria um novo status de subscrição")
            .WithDescription("Adiciona um novo status de subscrição ao banco de dados")
            .AddEndpointFilter<ValidationFilter<SubscriptionStatusDto>>()
            .Produces<Created<SubscriptionStatusResponseDto>>(StatusCodes.Status201Created)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);

        subscriptionStatuses.MapPut("/{id:int}", UpdateSubscriptionStatus)
            .WithSummary("Atualiza todos os dados de um status de subscrição")
            .WithDescription("Substitui todos os campos de um status de subscrição existente pelo ID")
            .AddEndpointFilter<ValidationFilter<SubscriptionStatusDto>>()
            .Produces<Ok<SubscriptionStatusResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptionStatuses.MapPatch("/{id:int}", PartialUpdateSubscriptionStatus)
            .WithSummary("Atualiza parcialmente um status de subscrição")
            .WithDescription("Modifica apenas os campos enviados no corpo da requisição")
            .AddEndpointFilter<ValidationFilter<PartialUpdateSubscriptionStatusDto>>()
            .Produces<Ok<SubscriptionStatusResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptionStatuses.MapDelete("/{id:int}", DeleteSubscriptionStatus)
            .WithSummary("Remove um status de subscrição")
            .WithDescription("Exclui um status de subscrição do sistema pelo ID informado")
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptionStatuses.MapGet("/search", SearchSubscriptionStatuses)
            .WithName("SearchSubscriptionStatuses")
            .WithSummary("Busca status de subscrição pela descrição")
            .WithDescription("Realiza uma busca textual nos status de subscrição cadastrados.")
            .Produces<Ok<PagedResponse<SubscriptionStatusResponseDto>>>(StatusCodes.Status200OK)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);
    }

    static async Task<Ok<PagedResponse<SubscriptionStatusResponseDto>>> GetAllSubscriptionStatuses(
        AppDbContext db,
        IHateoasLinkService hateoas,
        HttpContext http,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var totalSubscriptionStatuses = await db.SubscriptionStatuses.CountAsync();

        var subscriptionStatuses = await db.SubscriptionStatuses
            .OrderBy(f => f.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = subscriptionStatuses.Select(SubscriptionStatusResponseDto.FromEntity).ToList();

        foreach (var dto in dtos)
        {
            hateoas.AddItemLinks(
                dto,
                routeNameGetById: "GetSubscriptionStatusById",
                routeValuesForItem: new { id = dto.Id },
                extras:
                [
                    ("get_all_subscriptionStatuses", "GET", "GetAllSubscriptionStatuses", null),
                    ("create_subscriptionStatus", "POST", "CreateSubscriptionStatus", null),
                    ("update_subscriptionStatus", "PUT", "UpdateSubscriptionStatus", null),
                    ("partial_update_subscriptionStatus", "PATCH", "PartialUpdateSubscriptionStatus", null),
                    ("delete_subscriptionStatus", "DELETE", "DeleteSubscriptionStatus", null)
                ]
            );
        }

        var response = PaginationResponseBuilder.Build(
            items: dtos,
            page: page,
            pageSize: pageSize,
            totalItems: totalSubscriptionStatuses,
            routeBase: "subscriptionStatuses",
            http: http
        );

        return TypedResults.Ok(response);
    }

    static async Task<Results<Ok<SubscriptionStatusResponseDto>, NotFound>> GetSubscriptionStatusById(int id,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var subscriptionStatus = await db.SubscriptionStatuses.FindAsync(id);
        if (subscriptionStatus is null) return TypedResults.NotFound();

        var dto = SubscriptionStatusResponseDto.FromEntity(subscriptionStatus);
        hateoas.AddItemLinks(
            dto,
            routeNameGetById: "GetSubscriptionStatusById",
            routeValuesForItem: new { id = dto.Id },
            extras:
            [
                ("get_all_subscriptionStatuses", "GET", "GetAllSubscriptionStatuses", null),
                ("create_subscriptionStatus", "POST", "CreateSubscriptionStatus", null),
                ("update_subscriptionStatus", "PUT", "UpdateSubscriptionStatus", null),
                ("partial_update_subscriptionStatus", "PATCH", "PartialUpdateSubscriptionStatus", null),
                ("delete_subscriptionStatus", "DELETE", "DeleteSubscriptionStatus", null)
            ]
            );

        return TypedResults.Ok(dto);
    }

    static async Task<Results<Created<SubscriptionStatusResponseDto>, BadRequest>> CreateSubscriptionStatus(
        SubscriptionStatusDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        try
        {
            SubscriptionStatus newSubscriptionStatus = dto.ToEntity();

            await db.SubscriptionStatuses.AddAsync(newSubscriptionStatus);
            await db.SaveChangesAsync();

            var dtoResponse = SubscriptionStatusResponseDto.FromEntity(newSubscriptionStatus);
            hateoas.AddItemLinks(
                dtoResponse,
                routeNameGetById: "GetSubscriptionStatusById",
                routeValuesForItem: new { id = dtoResponse.Id },
                extras:
                [
                    ("get_all_subscriptionStatuses", "GET", "GetAllSubscriptionStatuses", null),
                    ("update_subscriptionStatus", "PUT", "UpdateSubscriptionStatus", null),
                    ("partial_update_subscriptionStatus", "PATCH", "PartialUpdateSubscriptionStatus", null),
                    ("delete_subscriptionStatus", "DELETE", "DeleteSubscriptionStatus", null)
                ]
            );

            return TypedResults.Created(
                $"/subscriptionStatuses/{newSubscriptionStatus.Id}",
                dtoResponse
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return TypedResults.BadRequest();
        }
    }

    static async Task<Results<Ok<SubscriptionStatusResponseDto>, NotFound>> UpdateSubscriptionStatus(int id,
        SubscriptionStatusDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var dbSubscriptionStatus = await db.SubscriptionStatuses.FindAsync(id);
        if (dbSubscriptionStatus is null)
            return TypedResults.NotFound();

        dto.ApplyToEntity(dbSubscriptionStatus);
        await db.SaveChangesAsync();

        var dtoResponse = SubscriptionStatusResponseDto.FromEntity(dbSubscriptionStatus);
        hateoas.AddItemLinks(
            dtoResponse,
            routeNameGetById: "GetSubscriptionStatusById",
            routeValuesForItem: new { id = dtoResponse.Id },
            extras:
            [
                ("get_all_subscriptionStatuses", "GET", "GetAllSubscriptionStatuses", null),
                ("create_subscriptionStatus", "POST", "CreateSubscriptionStatus", null),
                ("partial_update_subscriptionStatus", "PATCH", "PartialUpdateSubscriptionStatus", null),
                ("delete_subscriptionStatus", "DELETE", "DeleteSubscriptionStatus", null)
            ]
        );

        return TypedResults.Ok(dtoResponse);
    }

    static async Task<Results<Ok<SubscriptionStatusResponseDto>, NotFound>> PartialUpdateSubscriptionStatus(
        int id,
        PartialUpdateSubscriptionStatusDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var dbSubscriptionStatus = await db.SubscriptionStatuses.FindAsync(id);
        if (dbSubscriptionStatus is null)
            return TypedResults.NotFound();

        dto.ApplyToEntity(dbSubscriptionStatus);
        await db.SaveChangesAsync();

        var dtoResponse = SubscriptionStatusResponseDto.FromEntity(dbSubscriptionStatus);
        hateoas.AddItemLinks(
            dtoResponse,
            routeNameGetById: "GetSubscriptionStatusById",
            routeValuesForItem: new { id = dtoResponse.Id },
            extras:
            [
                ("get_all_subscriptionStatuses", "GET", "GetAllSubscriptionStatuses", null),
                ("create_subscriptionStatus", "POST", "CreateSubscriptionStatus", null),
                ("update_subscriptionStatus", "PUT", "UpdateSubscriptionStatus", null),
                ("delete_subscriptionStatus", "DELETE", "DeleteSubscriptionStatus", null)
            ]
        );

        return TypedResults.Ok(dtoResponse);
    }

    static async Task<Results<NoContent, NotFound>> DeleteSubscriptionStatus(int id, AppDbContext db, IHateoasLinkService hateoas)
    {
        var subscriptionStatus = await db.SubscriptionStatuses.FindAsync(id);
        if (subscriptionStatus is null)
            return TypedResults.NotFound();

        db.SubscriptionStatuses.Remove(subscriptionStatus);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<Ok<PagedResponse<SubscriptionStatusResponseDto>>, BadRequest>> SearchSubscriptionStatuses(
        string? q,
        AppDbContext db,
        IHateoasLinkService hateoas,
        HttpContext http,
        int page = 1,
        int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(q))
            return TypedResults.BadRequest();

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = db.SubscriptionStatuses
            .Where(f => EF.Functions.Like(f.Description, $"%{q}%"))
            .OrderBy(f => f.Id);

        var totalSubscriptionStatuses = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalSubscriptionStatuses / (double)pageSize);

        var subscriptionStatuses = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = subscriptionStatuses.Select(SubscriptionStatusResponseDto.FromEntity).ToList();

        foreach (var dto in dtos)
        {
            hateoas.AddItemLinks(
                dto,
                routeNameGetById: "GetSubscriptionStatusById",
                routeValuesForItem: new { id = dto.Id },
                extras:
                [
                    ("get_all_subscriptionStatuses", "GET", "GetAllSubscriptionStatuses", null),
                    ("create_subscriptionStatus", "POST", "CreateSubscriptionStatus", null),
                    ("update_subscriptionStatus", "PUT", "UpdateSubscriptionStatus", null),
                    ("delete_subscriptionStatus", "DELETE", "DeleteSubscriptionStatus", null)
                ]
            );
        }

        var response = PaginationResponseBuilder.Build(
            items: dtos,
            page: page,
            pageSize: pageSize,
            totalItems: totalSubscriptionStatuses,
            routeBase: $"subscriptionStatuses/search?q={q}",
            http: http
        );

        return TypedResults.Ok(response);
    }
}
