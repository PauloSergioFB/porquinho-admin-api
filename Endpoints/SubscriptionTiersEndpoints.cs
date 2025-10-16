using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PorquinhoApi.Data;
using PorquinhoApi.DTOs.Functionalities;
using PorquinhoApi.DTOs.SubscriptionTiers;
using PorquinhoApi.Filters;
using PorquinhoApi.Models;

namespace PorquinhoApi.Endpoints;

public static class SubscriptionTierEndpoints
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
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptionTiers.MapPatch("/{id:int}", PartialUpdateSubscriptionTier)
            .WithSummary("Atualiza parcialmente um nível de subscrição")
            .WithDescription("Modifica apenas os campos enviados no corpo da requisição")
            .AddEndpointFilter<ValidationFilter<PartialUpdateSubscriptionTierDto>>()
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptionTiers.MapDelete("/{id:int}", DeleteSubscriptionTier)
            .WithSummary("Remove um nível de subscrição")
            .WithDescription("Exclui um nível de subscrição do sistema pelo ID informado")
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptionTiers.MapGet("/{id:int}/functionalities", GetAllFunctionalitiesFromSubscriptionTierById)
            .WithSummary("Lista todas as funcionalidades vinculadas a um nível de subscrição")
            .WithDescription("Obtém todas as funcionalidades associadas ao nível de subscrição informado pelo ID")
            .Produces<Ok<List<FunctionalityResponseDto>>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);
    }

    static async Task<Ok<List<SubscriptionTierResponseDto>>> GetAllSubscriptionTiers(AppDbContext db)
    {
        var subscriptionTiers = await db.SubscriptionTiers
            .Include(t => t.Functionalities)
            .ToListAsync();

        return TypedResults.Ok(subscriptionTiers.Select(SubscriptionTierResponseDto.FromEntity).ToList());
    }

    static async Task<Results<Ok<SubscriptionTierResponseDto>, NotFound>> GetSubscriptionTierById(int id, AppDbContext db)
    {
        var subscriptionTier = await db.SubscriptionTiers
            .Include(t => t.Functionalities)
            .FirstOrDefaultAsync(t => t.Id == id);

        return subscriptionTier is not null
            ? TypedResults.Ok(SubscriptionTierResponseDto.FromEntity(subscriptionTier))
            : TypedResults.NotFound();
    }

    static async Task<Results<Created<SubscriptionTierResponseDto>, BadRequest>> CreateSubscriptionTier(SubscriptionTierDto dto, AppDbContext db)
    {
        try
        {
            var newSubscriptionTier = dto.ToEntity();

            if (dto.FunctionalityIds is not null && dto.FunctionalityIds.Count != 0)
            {
                var functionalities = await db.Functionalities
                    .Where(f => dto.FunctionalityIds.Contains(f.Id))
                    .ToListAsync();

                newSubscriptionTier.Functionalities = functionalities;
            }

            await db.SubscriptionTiers.AddAsync(newSubscriptionTier);
            await db.SaveChangesAsync();

            return TypedResults.Created(
                $"/subscription-tiers/{newSubscriptionTier.Id}",
                SubscriptionTierResponseDto.FromEntity(newSubscriptionTier)
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return TypedResults.BadRequest();
        }
    }

    static async Task<Results<NoContent, NotFound>> UpdateSubscriptionTier(int id, SubscriptionTierDto dto, AppDbContext db)
    {
        var dbSubscriptionTier = await db.SubscriptionTiers
            .Include(t => t.Functionalities)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (dbSubscriptionTier is null)
            return TypedResults.NotFound();

        if (dto.FunctionalityIds is not null && dto.FunctionalityIds.Count != 0)
        {
            var functionalities = await db.Functionalities
                .Where(f => dto.FunctionalityIds.Contains(f.Id))
                .ToListAsync();

            dbSubscriptionTier.Functionalities = functionalities;
        }

        SubscriptionTier updatedSubscriptionTier = dto.ToEntity();

        updatedSubscriptionTier.Id = id;

        dto.ApplyToEntity(dbSubscriptionTier);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound>> PartialUpdateSubscriptionTier(int id, PartialUpdateSubscriptionTierDto dto, AppDbContext db)
    {
        var dbSubscriptionTier = await db.SubscriptionTiers
            .Include(t => t.Functionalities)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (dbSubscriptionTier is null)
            return TypedResults.NotFound();

        if (dto.FunctionalityIds is not null && dto.FunctionalityIds.Count != 0)
        {
            var functionalities = await db.Functionalities
                .Where(f => dto.FunctionalityIds.Contains(f.Id))
                .ToListAsync();

            dbSubscriptionTier.Functionalities = functionalities;
        }

        dto.ApplyToEntity(dbSubscriptionTier);

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound>> DeleteSubscriptionTier(int id, AppDbContext db)
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
}