using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PorquinhoApi.Data;
using PorquinhoApi.DTOs.SubscriptionStatuses;
using PorquinhoApi.Filters;
using PorquinhoApi.Models;

namespace PorquinhoApi.Endpoints;

public static class SubscriptionStatusEndpoints
{
    public static void MapSubscriptionStatusEndpoints(this WebApplication app)
    {
        var subscriptionStatuses = app.MapGroup("/subscription-statuses").WithTags("SubscriptionStatuses");

        subscriptionStatuses.MapGet("/", GetAllSubscriptionStatuses)
            .WithSummary("Retorna todos os status de subscrição cadastrados")
            .WithDescription("Obtém uma lista completa de status de subscrição do sistema")
            .Produces<Ok<List<SubscriptionStatusResponseDto>>>(StatusCodes.Status200OK);

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
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptionStatuses.MapPatch("/{id:int}", PartialUpdateSubscriptionStatus)
            .WithSummary("Atualiza parcialmente um status de subscrição")
            .WithDescription("Modifica apenas os campos enviados no corpo da requisição")
            .AddEndpointFilter<ValidationFilter<PartialUpdateSubscriptionStatusDto>>()
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        subscriptionStatuses.MapDelete("/{id:int}", DeleteSubscriptionStatus)
            .WithSummary("Remove um status de subscrição")
            .WithDescription("Exclui um status de subscrição do sistema pelo ID informado")
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);
    }

    static async Task<Ok<List<SubscriptionStatusResponseDto>>> GetAllSubscriptionStatuses(AppDbContext db)
    {
        var subscriptionStatuses = await db.SubscriptionStatuses.ToListAsync();
        return TypedResults.Ok(subscriptionStatuses.Select(SubscriptionStatusResponseDto.FromEntity).ToList());
    }

    static async Task<Results<Ok<SubscriptionStatusResponseDto>, NotFound>> GetSubscriptionStatusById(int id, AppDbContext db)
    {
        var subscriptionStatus = await db.SubscriptionStatuses.FindAsync(id);
        return subscriptionStatus is not null
            ? TypedResults.Ok(SubscriptionStatusResponseDto.FromEntity(subscriptionStatus))
            : TypedResults.NotFound();
    }

    static async Task<Results<Created<SubscriptionStatusResponseDto>, BadRequest>> CreateSubscriptionStatus(SubscriptionStatusDto dto, AppDbContext db)
    {
        try
        {
            SubscriptionStatus newSubscriptionStatus = dto.ToEntity();

            await db.SubscriptionStatuses.AddAsync(newSubscriptionStatus);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/subscription-statuses/{newSubscriptionStatus.Id}", SubscriptionStatusResponseDto.FromEntity(newSubscriptionStatus));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return TypedResults.BadRequest();
        }
    }

    static async Task<Results<NoContent, NotFound>> UpdateSubscriptionStatus(int id, SubscriptionStatusDto dto, AppDbContext db)
    {
        var dbSubscriptionStatus = await db.SubscriptionStatuses.FindAsync(id);
        if (dbSubscriptionStatus is null)
            return TypedResults.NotFound();

        SubscriptionStatus updatedSubscriptionStatus = dto.ToEntity();

        updatedSubscriptionStatus.Id = id;

        dto.ApplyToEntity(dbSubscriptionStatus);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound>> PartialUpdateSubscriptionStatus(int id, PartialUpdateSubscriptionStatusDto dto, AppDbContext db)
    {
        var dbSubscriptionStatus = await db.SubscriptionStatuses.FindAsync(id);
        if (dbSubscriptionStatus is null)
            return TypedResults.NotFound();

        dto.ApplyToEntity(dbSubscriptionStatus);

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound>> DeleteSubscriptionStatus(int id, AppDbContext db)
    {
        var subscriptionStatus = await db.SubscriptionStatuses.FindAsync(id);
        if (subscriptionStatus is null)
            return TypedResults.NotFound();

        db.SubscriptionStatuses.Remove(subscriptionStatus);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}