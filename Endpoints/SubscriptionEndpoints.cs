using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PorquinhoApi.Data;
using PorquinhoApi.DTOs.Subscriptions;
using PorquinhoApi.Filters;
using PorquinhoApi.Models;
using PorquinhoApi.Services;
using PorquinhoApi.Utils;

namespace PorquinhoApi.Endpoints;

public static class SubscriptionsEndpoints
{
    public static void MapSubscriptionEndpoints(this WebApplication app)
    {
        var subscriptions = app.MapGroup("/subscriptions").WithTags("Subscriptions");

        subscriptions.MapGet("/", GetAllSubscriptions)
            .WithName("GetAllSubscriptions")
            .WithSummary("Retorna todos as subscrições cadastradas")
            .WithDescription("Obtém uma lista completa de subscrições do sistema")
            .Produces<Ok<PagedResponse<SubscriptionResponseDto>>>(StatusCodes.Status200OK);

        subscriptions.MapGet("/{id:int}", GetSubscriptionById)
            .WithName("GetSubscriptionById")
            .WithSummary("Retorna uma subscrição específico")
            .WithDescription("Obtém uma subscrição pelo ID informado")
            .Produces<Ok<SubscriptionResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptions.MapPost("/", CreateSubscription)
            .WithName("CreateSubscription")
            .WithSummary("Cria uma nova subscrição")
            .WithDescription("Adiciona uma nova subscrição ao banco de dados")
            .AddEndpointFilter<ValidationFilter<SubscriptionDto>>()
            .Produces<Created<SubscriptionResponseDto>>(StatusCodes.Status201Created)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);

        subscriptions.MapPut("/{id:int}", UpdateSubscription)
            .WithName("UpdateSubscription")
            .WithSummary("Atualiza todos os dados de uma subscrição")
            .WithDescription("Substitui todos os campos de uma subscrição existente pelo ID")
            .AddEndpointFilter<ValidationFilter<SubscriptionDto>>()
            .Produces<Ok<SubscriptionResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptions.MapPatch("/{id:int}", PartialUpdateSubscription)
            .WithName("PartialUpdateSubscription")
            .WithSummary("Atualiza parcialmente uma subscrição")
            .WithDescription("Modifica apenas os campos enviados no corpo da requisição")
            .AddEndpointFilter<ValidationFilter<PartialUpdateSubscriptionDto>>()
            .Produces<Ok<SubscriptionResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptions.MapDelete("/{id:int}", DeleteSubscription)
            .WithName("DeleteSubscription")
            .WithSummary("Remove uma subscrição")
            .WithDescription("Exclui uma subscrição do sistema pelo ID informado")
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptions.MapGet("/search", SearchSubscriptions)
            .WithName("SearchSubscriptions")
            .WithSummary("Busca subscrições pelo id do usuário")
            .WithDescription("Realiza uma busca pelo id do usuário nas subscrições cadastradas.")
            .Produces<Ok<PagedResponse<SubscriptionResponseDto>>>(StatusCodes.Status200OK)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);
    }

    static async Task<Ok<PagedResponse<SubscriptionResponseDto>>> GetAllSubscriptions(
        AppDbContext db,
        IHateoasLinkService hateoas,
        HttpContext http,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var totalSubscriptions = await db.Subscriptions.CountAsync();

        var subscriptions = await db.Subscriptions
            .OrderBy(f => f.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = subscriptions.Select(SubscriptionResponseDto.FromEntity).ToList();

        foreach (var dto in dtos)
        {
            hateoas.AddItemLinks(
                dto,
                routeNameGetById: "GetSubscriptionById",
                routeValuesForItem: new { id = dto.Id },
                extras:
                [
                    ("get_all_subscriptions", "GET", "GetAllSubscriptions", null),
                    ("create_subscription", "POST", "CreateSubscription", null),
                    ("update_subscription", "PUT", "UpdateSubscription", null),
                    ("partial_update_subscription", "PATCH", "PartialUpdateSubscription", null),
                    ("delete_subscription", "DELETE", "DeleteSubscription", null)
                ]
            );
        }

        var response = PaginationResponseBuilder.Build(
            items: dtos,
            page: page,
            pageSize: pageSize,
            totalItems: totalSubscriptions,
            routeBase: "subscriptions",
            http: http
        );

        return TypedResults.Ok(response);
    }

    static async Task<Results<Ok<SubscriptionResponseDto>, NotFound>> GetSubscriptionById(int id,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var user = await db.Subscriptions.FindAsync(id);
        if (user is null) return TypedResults.NotFound();

        var dto = SubscriptionResponseDto.FromEntity(user);
        hateoas.AddItemLinks(
            dto,
            routeNameGetById: "GetSubscriptionById",
            routeValuesForItem: new { id = dto.Id },
            extras:
            [
                ("get_all_subscriptions", "GET", "GetAllSubscriptions", null),
                ("create_subscription", "POST", "CreateSubscription", null),
                ("update_subscription", "PUT", "UpdateSubscription", null),
                ("partial_update_subscription", "PATCH", "PartialUpdateSubscription", null),
                ("delete_subscription", "DELETE", "DeleteSubscription", null)
            ]
            );

        return TypedResults.Ok(dto);
    }

    static async Task<Results<Created<SubscriptionResponseDto>, BadRequest>> CreateSubscription(
        SubscriptionDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        try
        {
            Subscription newSubscription = dto.ToEntity();

            await db.Subscriptions.AddAsync(newSubscription);
            await db.SaveChangesAsync();

            var dtoResponse = SubscriptionResponseDto.FromEntity(newSubscription);
            hateoas.AddItemLinks(
                dtoResponse,
                routeNameGetById: "GetSubscriptionById",
                routeValuesForItem: new { id = dtoResponse.Id },
                extras:
                [
                    ("get_all_subscriptions", "GET", "GetAllSubscriptions", null),
                    ("update_subscription", "PUT", "UpdateSubscription", null),
                    ("partial_update_subscription", "PATCH", "PartialUpdateSubscription", null),
                    ("delete_subscription", "DELETE", "DeleteSubscription", null)
                ]
            );

            return TypedResults.Created(
                $"/subscriptions/{newSubscription.Id}",
                dtoResponse
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return TypedResults.BadRequest();
        }
    }

    static async Task<Results<Ok<SubscriptionResponseDto>, NotFound>> UpdateSubscription(int id,
        SubscriptionDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var dbSubscription = await db.Subscriptions.FindAsync(id);
        if (dbSubscription is null)
            return TypedResults.NotFound();

        dto.ApplyToEntity(dbSubscription);
        await db.SaveChangesAsync();

        var dtoResponse = SubscriptionResponseDto.FromEntity(dbSubscription);
        hateoas.AddItemLinks(
            dtoResponse,
            routeNameGetById: "GetSubscriptionById",
            routeValuesForItem: new { id = dtoResponse.Id },
            extras:
            [
                ("get_all_subscriptions", "GET", "GetAllSubscriptions", null),
                ("create_subscription", "POST", "CreateSubscription", null),
                ("partial_update_subscription", "PATCH", "PartialUpdateSubscription", null),
                ("delete_subscription", "DELETE", "DeleteSubscription", null)
            ]
        );

        return TypedResults.Ok(dtoResponse);
    }

    static async Task<Results<Ok<SubscriptionResponseDto>, NotFound>> PartialUpdateSubscription(
        int id,
        PartialUpdateSubscriptionDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var dbSubscription = await db.Subscriptions.FindAsync(id);
        if (dbSubscription is null)
            return TypedResults.NotFound();

        dto.ApplyToEntity(dbSubscription);
        await db.SaveChangesAsync();

        var dtoResponse = SubscriptionResponseDto.FromEntity(dbSubscription);
        hateoas.AddItemLinks(
            dtoResponse,
            routeNameGetById: "GetSubscriptionById",
            routeValuesForItem: new { id = dtoResponse.Id },
            extras:
            [
                ("get_all_subscriptions", "GET", "GetAllSubscriptions", null),
                ("create_subscription", "POST", "CreateSubscription", null),
                ("update_subscription", "PUT", "UpdateSubscription", null),
                ("delete_subscription", "DELETE", "DeleteSubscription", null)
            ]
        );

        return TypedResults.Ok(dtoResponse);
    }

    static async Task<Results<NoContent, NotFound>> DeleteSubscription(int id, AppDbContext db, IHateoasLinkService hateoas)
    {
        var user = await db.Subscriptions.FindAsync(id);
        if (user is null)
            return TypedResults.NotFound();

        db.Subscriptions.Remove(user);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<Ok<PagedResponse<SubscriptionResponseDto>>, BadRequest>> SearchSubscriptions(
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

        _ = int.TryParse(q, out var idValue);
        var query = db.Subscriptions
            .Where(f => f.UserId == idValue)
            .OrderBy(f => f.Id);

        var totalSubscriptions = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalSubscriptions / (double)pageSize);

        var subscriptions = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = subscriptions.Select(SubscriptionResponseDto.FromEntity).ToList();

        foreach (var dto in dtos)
        {
            hateoas.AddItemLinks(
                dto,
                routeNameGetById: "GetSubscriptionById",
                routeValuesForItem: new { id = dto.Id },
                extras:
                [
                    ("get_all_subscriptions", "GET", "GetAllSubscriptions", null),
                    ("create_subscription", "POST", "CreateSubscription", null),
                    ("update_subscription", "PUT", "UpdateSubscription", null),
                    ("delete_subscription", "DELETE", "DeleteSubscription", null)
                ]
            );
        }

        var response = PaginationResponseBuilder.Build(
            items: dtos,
            page: page,
            pageSize: pageSize,
            totalItems: totalSubscriptions,
            routeBase: $"subscriptions/search?q={q}",
            http: http
        );

        return TypedResults.Ok(response);
    }
}
