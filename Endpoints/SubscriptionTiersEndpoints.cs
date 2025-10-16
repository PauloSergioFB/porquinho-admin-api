using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PorquinhoApi.Data;
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
    }

    static async Task<Ok<List<SubscriptionTierResponseDto>>> GetAllSubscriptionTiers(AppDbContext db)
    {
        var subscriptionTiers = await db.SubscriptionTiers.ToListAsync();
        return TypedResults.Ok(subscriptionTiers.Select(SubscriptionTierResponseDto.FromEntity).ToList());
    }

    static async Task<Results<Ok<SubscriptionTierResponseDto>, NotFound>> GetSubscriptionTierById(int id, AppDbContext db)
    {
        var subscriptionTier = await db.SubscriptionTiers.FindAsync(id);
        return subscriptionTier is not null
            ? TypedResults.Ok(SubscriptionTierResponseDto.FromEntity(subscriptionTier))
            : TypedResults.NotFound();
    }

    static async Task<Results<Created<SubscriptionTierResponseDto>, BadRequest>> CreateSubscriptionTier(SubscriptionTierDto dto, AppDbContext db)
    {
        try
        {
            SubscriptionTier newSubscriptionTier = dto.ToEntity();

            await db.SubscriptionTiers.AddAsync(newSubscriptionTier);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/subscriptionTiers/{newSubscriptionTier.Id}", SubscriptionTierResponseDto.FromEntity(newSubscriptionTier));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return TypedResults.BadRequest();
        }
    }

    static async Task<Results<NoContent, NotFound>> UpdateSubscriptionTier(int id, SubscriptionTierDto dto, AppDbContext db)
    {
        var dbSubscriptionTier = await db.SubscriptionTiers.FindAsync(id);
        if (dbSubscriptionTier is null)
            return TypedResults.NotFound();

        SubscriptionTier updatedSubscriptionTier = dto.ToEntity();

        updatedSubscriptionTier.Id = id;

        dto.ApplyToEntity(dbSubscriptionTier);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound>> PartialUpdateSubscriptionTier(int id, PartialUpdateSubscriptionTierDto dto, AppDbContext db)
    {
        var dbSubscriptionTier = await db.SubscriptionTiers.FindAsync(id);
        if (dbSubscriptionTier is null)
            return TypedResults.NotFound();

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
}