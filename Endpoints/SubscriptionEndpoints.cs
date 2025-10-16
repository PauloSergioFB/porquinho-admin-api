using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PorquinhoApi.Data;
using PorquinhoApi.DTOs.Subscriptions;
using PorquinhoApi.Filters;
using PorquinhoApi.Models;

namespace PorquinhoApi.Endpoints;

public static class SubscriptionEndpoints
{
    public static void MapSubscriptionEndpoints(this WebApplication app)
    {
        var subscriptions = app.MapGroup("/subscriptions").WithTags("Subscriptions");

        subscriptions.MapGet("/", GetAllSubscriptions)
            .WithSummary("Retorna todos as subscrições cadastradas")
            .WithDescription("Obtém uma lista completa de subscrições do sistema")
            .Produces<Ok<List<SubscriptionResponseDto>>>(StatusCodes.Status200OK);

        subscriptions.MapGet("/{id:int}", GetSubscriptionById)
            .WithSummary("Retorna uma subscrição específico")
            .WithDescription("Obtém uma subscrição pelo ID informado")
            .Produces<Ok<SubscriptionResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptions.MapPost("/", CreateSubscription)
            .WithSummary("Cria uma nova subscrição")
            .WithDescription("Adiciona uma nova subscrição ao banco de dados")
            .AddEndpointFilter<ValidationFilter<SubscriptionDto>>()
            .Produces<Created<SubscriptionResponseDto>>(StatusCodes.Status201Created)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);

        subscriptions.MapPut("/{id:int}", UpdateSubscription)
            .WithSummary("Atualiza todos os dados de uma subscrição")
            .WithDescription("Substitui todos os campos de uma subscrição existente pelo ID")
            .AddEndpointFilter<ValidationFilter<SubscriptionDto>>()
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptions.MapPatch("/{id:int}", PartialUpdateSubscription)
            .WithSummary("Atualiza parcialmente uma subscrição")
            .WithDescription("Modifica apenas os campos enviados no corpo da requisição")
            .AddEndpointFilter<ValidationFilter<PartialUpdateSubscriptionDto>>()
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptions.MapDelete("/{id:int}", DeleteSubscription)
            .WithSummary("Remove uma subscrição")
            .WithDescription("Exclui uma subscrição do sistema pelo ID informado")
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);
    }

    static async Task<Ok<List<SubscriptionResponseDto>>> GetAllSubscriptions(AppDbContext db)
    {
        var subscriptions = await db.Subscriptions.ToListAsync();
        return TypedResults.Ok(subscriptions.Select(SubscriptionResponseDto.FromEntity).ToList());
    }

    static async Task<Results<Ok<SubscriptionResponseDto>, NotFound>> GetSubscriptionById(int id, AppDbContext db)
    {
        var subscription = await db.Subscriptions.FindAsync(id);
        return subscription is not null
            ? TypedResults.Ok(SubscriptionResponseDto.FromEntity(subscription))
            : TypedResults.NotFound();
    }

    static async Task<Results<Created<SubscriptionResponseDto>, BadRequest>> CreateSubscription(SubscriptionDto dto, AppDbContext db)
    {
        try
        {
            Subscription newSubscription = dto.ToEntity();
            newSubscription.CreatedAt = DateTime.UtcNow;

            await db.Subscriptions.AddAsync(newSubscription);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/subscriptions/{newSubscription.Id}", SubscriptionResponseDto.FromEntity(newSubscription));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return TypedResults.BadRequest();
        }
    }

    static async Task<Results<NoContent, NotFound>> UpdateSubscription(int id, SubscriptionDto dto, AppDbContext db)
    {
        var dbSubscription = await db.Subscriptions.FindAsync(id);
        if (dbSubscription is null)
            return TypedResults.NotFound();

        Subscription updatedSubscription = dto.ToEntity();

        updatedSubscription.Id = id;
        updatedSubscription.UpdatedAt = DateTime.UtcNow;

        dto.ApplyToEntity(dbSubscription);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound>> PartialUpdateSubscription(int id, PartialUpdateSubscriptionDto dto, AppDbContext db)
    {
        var dbSubscription = await db.Subscriptions.FindAsync(id);
        if (dbSubscription is null)
            return TypedResults.NotFound();

        dto.ApplyToEntity(dbSubscription);

        dbSubscription.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound>> DeleteSubscription(int id, AppDbContext db)
    {
        var subscription = await db.Subscriptions.FindAsync(id);
        if (subscription is null)
            return TypedResults.NotFound();

        db.Subscriptions.Remove(subscription);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}