using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PorquinhoApi.Data;
using PorquinhoApi.DTOs.Functionalities;
using PorquinhoApi.DTOs.SubscriptionTiers;
using PorquinhoApi.Filters;
using PorquinhoApi.Models;
using PorquinhoApi.Services;
using PorquinhoApi.Utils;

namespace PorquinhoApi.Endpoints;

public static class SubscriptionTiersEndpoints
{
    public static void MapSubscriptionTierEndpoints(this WebApplication app)
    {
        var subscriptionTiers = app.MapGroup("/subscription-tiers").WithTags("SubscriptionTiers");

        subscriptionTiers.MapGet("/", GetAllSubscriptionTiers)
            .WithSummary("Retorna todos os níveis de subscrição")
            .WithDescription("Obtém uma lista completa de níveis de subscrição do sistema")
            .Produces<Ok<List<SubscriptionTierResponseDto>>>(StatusCodes.Status200OK);

        subscriptionTiers.MapGet("/{id:int}", GetSubscriptionTierById)
            .WithSummary("Retorna um nível de subscrição específico")
            .WithDescription("Obtém um nível de subscrição pelo ID informado")
            .Produces<Ok<SubscriptionTierResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptionTiers.MapPost("/", CreateSubscriptionTier)
            .WithSummary("Cria um novo nível de subscrição")
            .WithDescription("Adiciona um novo nível de subscrição ao banco de dados")
            .AddEndpointFilter<ValidationFilter<SubscriptionTierDto>>()
            .Produces<Created<SubscriptionTierResponseDto>>(StatusCodes.Status201Created)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);

        subscriptionTiers.MapPut("/{id:int}", UpdateSubscriptionTier)
            .WithSummary("Atualiza todos os dados de um nível de subscrição")
            .WithDescription("Substitui todos os campos de um nível de subscrição existente pelo ID")
            .AddEndpointFilter<ValidationFilter<SubscriptionTierDto>>()
            .Produces<Ok<SubscriptionTierResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptionTiers.MapPatch("/{id:int}", PartialUpdateSubscriptionTier)
            .WithSummary("Atualiza parcialmente um nível de subscrição")
            .WithDescription("Modifica apenas os campos enviados no corpo da requisição")
            .AddEndpointFilter<ValidationFilter<PartialUpdateSubscriptionTierDto>>()
            .Produces<Ok<SubscriptionTierResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptionTiers.MapDelete("/{id:int}", DeleteSubscriptionTier)
            .WithSummary("Remove um nível de subscrição")
            .WithDescription("Exclui um nível de subscrição do sistema pelo ID informado")
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptionTiers.MapGet("/{id:int}/functionalities", GetAllFunctionalitiesFromSubscriptionTierById)
            .WithSummary("Lista todas as funcionalidades vinculadas a um nível de subscrição")
            .WithDescription("Obtém todas as funcionalidades associadas ao nível de subscrição informado pelo ID")
            .Produces<Ok<List<SubscriptionTierResponseDto>>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptionTiers.MapGet("/search", SearchSubscriptionTiers)
            .WithName("SearchSubscriptionTiers")
            .WithSummary("Busca níveis de subscrição pelo nome")
            .WithDescription("Realiza uma busca textual nos níveis de subscrição cadastrados.")
            .Produces<Ok<PagedResponse<SubscriptionTierResponseDto>>>(StatusCodes.Status200OK)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);
    }

    static async Task<Ok<PagedResponse<SubscriptionTierResponseDto>>> GetAllSubscriptionTiers(
        AppDbContext db,
        IHateoasLinkService hateoas,
        HttpContext http,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var totalSubscriptionTiers = await db.SubscriptionTiers.CountAsync();

        var subscriptionTiers = await db.SubscriptionTiers
            .OrderBy(f => f.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = subscriptionTiers.Select(SubscriptionTierResponseDto.FromEntity).ToList();

        foreach (var dto in dtos)
        {
            hateoas.AddItemLinks(
                dto,
                routeNameGetById: "GetSubscriptionTierById",
                routeValuesForItem: new { id = dto.Id },
                extras:
                [
                    ("get_all_subscriptionTiers", "GET", "GetAllSubscriptionTiers", null),
                    ("create_subscriptionTier", "POST", "CreateSubscriptionTier", null),
                    ("update_subscriptionTier", "PUT", "UpdateSubscriptionTier", null),
                    ("partial_update_subscriptionTier", "PATCH", "PartialUpdateSubscriptionTier", null),
                    ("delete_subscriptionTier", "DELETE", "DeleteSubscriptionTier", null)
                ]
            );
        }

        var response = PaginationResponseBuilder.Build(
            items: dtos,
            page: page,
            pageSize: pageSize,
            totalItems: totalSubscriptionTiers,
            routeBase: "subscriptionTiers",
            http: http
        );

        return TypedResults.Ok(response);
    }

    static async Task<Results<Ok<SubscriptionTierResponseDto>, NotFound>> GetSubscriptionTierById(int id,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var subscriptionTier = await db.SubscriptionTiers.FindAsync(id);
        if (subscriptionTier is null) return TypedResults.NotFound();

        var dto = SubscriptionTierResponseDto.FromEntity(subscriptionTier);
        hateoas.AddItemLinks(
            dto,
            routeNameGetById: "GetSubscriptionTierById",
            routeValuesForItem: new { id = dto.Id },
            extras:
            [
                ("get_all_subscriptionTiers", "GET", "GetAllSubscriptionTiers", null),
                ("create_subscriptionTier", "POST", "CreateSubscriptionTier", null),
                ("update_subscriptionTier", "PUT", "UpdateSubscriptionTier", null),
                ("partial_update_subscriptionTier", "PATCH", "PartialUpdateSubscriptionTier", null),
                ("delete_subscriptionTier", "DELETE", "DeleteSubscriptionTier", null)
            ]
            );

        return TypedResults.Ok(dto);
    }

    static async Task<Results<Created<SubscriptionTierResponseDto>, BadRequest>> CreateSubscriptionTier(
        SubscriptionTierDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        try
        {
            SubscriptionTier newSubscriptionTier = dto.ToEntity();

            await db.SubscriptionTiers.AddAsync(newSubscriptionTier);
            await db.SaveChangesAsync();

            var dtoResponse = SubscriptionTierResponseDto.FromEntity(newSubscriptionTier);
            hateoas.AddItemLinks(
                dtoResponse,
                routeNameGetById: "GetSubscriptionTierById",
                routeValuesForItem: new { id = dtoResponse.Id },
                extras:
                [
                    ("get_all_subscriptionTiers", "GET", "GetAllSubscriptionTiers", null),
                    ("update_subscriptionTier", "PUT", "UpdateSubscriptionTier", null),
                    ("partial_update_subscriptionTier", "PATCH", "PartialUpdateSubscriptionTier", null),
                    ("delete_subscriptionTier", "DELETE", "DeleteSubscriptionTier", null)
                ]
            );

            return TypedResults.Created(
                $"/subscriptionTiers/{newSubscriptionTier.Id}",
                dtoResponse
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return TypedResults.BadRequest();
        }
    }

    static async Task<Results<Ok<SubscriptionTierResponseDto>, NotFound>> UpdateSubscriptionTier(int id,
        SubscriptionTierDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var dbSubscriptionTier = await db.SubscriptionTiers.FindAsync(id);
        if (dbSubscriptionTier is null)
            return TypedResults.NotFound();

        dto.ApplyToEntity(dbSubscriptionTier);
        await db.SaveChangesAsync();

        var dtoResponse = SubscriptionTierResponseDto.FromEntity(dbSubscriptionTier);
        hateoas.AddItemLinks(
            dtoResponse,
            routeNameGetById: "GetSubscriptionTierById",
            routeValuesForItem: new { id = dtoResponse.Id },
            extras:
            [
                ("get_all_subscriptionTiers", "GET", "GetAllSubscriptionTiers", null),
                ("create_subscriptionTier", "POST", "CreateSubscriptionTier", null),
                ("partial_update_subscriptionTier", "PATCH", "PartialUpdateSubscriptionTier", null),
                ("delete_subscriptionTier", "DELETE", "DeleteSubscriptionTier", null)
            ]
        );

        return TypedResults.Ok(dtoResponse);
    }

    static async Task<Results<Ok<SubscriptionTierResponseDto>, NotFound>> PartialUpdateSubscriptionTier(
        int id,
        PartialUpdateSubscriptionTierDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var dbSubscriptionTier = await db.SubscriptionTiers.FindAsync(id);
        if (dbSubscriptionTier is null)
            return TypedResults.NotFound();

        dto.ApplyToEntity(dbSubscriptionTier);
        await db.SaveChangesAsync();

        var dtoResponse = SubscriptionTierResponseDto.FromEntity(dbSubscriptionTier);
        hateoas.AddItemLinks(
            dtoResponse,
            routeNameGetById: "GetSubscriptionTierById",
            routeValuesForItem: new { id = dtoResponse.Id },
            extras:
            [
                ("get_all_subscriptionTiers", "GET", "GetAllSubscriptionTiers", null),
                ("create_subscriptionTier", "POST", "CreateSubscriptionTier", null),
                ("update_subscriptionTier", "PUT", "UpdateSubscriptionTier", null),
                ("delete_subscriptionTier", "DELETE", "DeleteSubscriptionTier", null)
            ]
        );

        return TypedResults.Ok(dtoResponse);
    }

    static async Task<Results<NoContent, NotFound>> DeleteSubscriptionTier(int id, AppDbContext db, IHateoasLinkService hateoas)
    {
        var subscriptionTier = await db.SubscriptionTiers.FindAsync(id);
        if (subscriptionTier is null)
            return TypedResults.NotFound();

        db.SubscriptionTiers.Remove(subscriptionTier);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<Ok<List<FunctionalityResponseDto>>, NotFound>> GetAllFunctionalitiesFromSubscriptionTierById(int id, AppDbContext db)
    {
        var subscriptionTier = await db.SubscriptionTiers
            .Include(t => t.Functionalities)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (subscriptionTier is null)
            return TypedResults.NotFound();

        var functionalities = subscriptionTier.Functionalities
            .Select(FunctionalityResponseDto.FromEntity)
            .ToList();

        return TypedResults.Ok(functionalities);
    }

    static async Task<Results<Ok<PagedResponse<SubscriptionTierResponseDto>>, BadRequest>> SearchSubscriptionTiers(
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

        var query = db.SubscriptionTiers
            .Where(f => EF.Functions.Like(f.Name, $"%{q}%"))
            .OrderBy(f => f.Id);

        var totalSubscriptionTiers = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalSubscriptionTiers / (double)pageSize);

        var subscriptionTiers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = subscriptionTiers.Select(SubscriptionTierResponseDto.FromEntity).ToList();

        foreach (var dto in dtos)
        {
            hateoas.AddItemLinks(
                dto,
                routeNameGetById: "GetSubscriptionTierById",
                routeValuesForItem: new { id = dto.Id },
                extras:
                [
                    ("get_all_subscriptionTiers", "GET", "GetAllSubscriptionTiers", null),
                    ("create_subscriptionTier", "POST", "CreateSubscriptionTier", null),
                    ("update_subscriptionTier", "PUT", "UpdateSubscriptionTier", null),
                    ("delete_subscriptionTier", "DELETE", "DeleteSubscriptionTier", null)
                ]
            );
        }

        var response = PaginationResponseBuilder.Build(
            items: dtos,
            page: page,
            pageSize: pageSize,
            totalItems: totalSubscriptionTiers,
            routeBase: $"subscriptionTiers/search?q={q}",
            http: http
        );

        return TypedResults.Ok(response);
    }
}
