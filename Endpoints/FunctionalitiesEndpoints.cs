using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PorquinhoApi.Data;
using PorquinhoApi.DTOs.Functionalities;
using PorquinhoApi.Filters;
using PorquinhoApi.Models;

namespace PorquinhoApi.Endpoints;

public static class FunctionalitiesEndpoints
{
    public static void MapFunctionalitiesEndpoints(this WebApplication app)
    {
        var functionalities = app.MapGroup("/functionalities").WithTags("Functionalities");

        functionalities.MapGet("/", GetAllFunctionalities)
            .WithSummary("Retorna todos as funcionalidades cadastradas")
            .WithDescription("Obtém uma lista completa de funcionalidades do sistema")
            .Produces<Ok<List<FunctionalityResponseDto>>>(StatusCodes.Status200OK);

        functionalities.MapGet("/{id:int}", GetFunctionalityById)
            .WithSummary("Retorna uma funcionalidade específica")
            .WithDescription("Obtém uma funcionalidade pelo ID informado")
            .Produces<Ok<FunctionalityResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        functionalities.MapPost("/", CreateFunctionality)
            .WithSummary("Cria uma nova funcionalidade")
            .WithDescription("Adiciona uma nova funcionalidade ao banco de dados")
            .AddEndpointFilter<ValidationFilter<FunctionalityDto>>()
            .Produces<Created<FunctionalityResponseDto>>(StatusCodes.Status201Created)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);

        functionalities.MapPut("/{id:int}", UpdateFunctionality)
            .WithSummary("Atualiza todos os dados de uma funcionalidade")
            .WithDescription("Substitui todos os campos de uma funcionalidade existente pelo ID")
            .AddEndpointFilter<ValidationFilter<FunctionalityDto>>()
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        functionalities.MapPatch("/{id:int}", PartialUpdateFunctionality)
            .WithSummary("Atualiza parcialmente uma funcionalidade")
            .WithDescription("Modifica apenas os campos enviados no corpo da requisição")
            .AddEndpointFilter<ValidationFilter<PartialUpdateFunctionalityDto>>()
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        functionalities.MapDelete("/{id:int}", DeleteFunctionality)
            .WithSummary("Remove uma funcionalidade")
            .WithDescription("Exclui uma funcionalidade do sistema pelo ID informado")
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);
    }

    static async Task<Ok<List<FunctionalityResponseDto>>> GetAllFunctionalities(AppDbContext db)
    {
        var functionalities = await db.Functionalities.ToListAsync();
        return TypedResults.Ok(functionalities.Select(FunctionalityResponseDto.FromEntity).ToList());
    }

    static async Task<Results<Ok<FunctionalityResponseDto>, NotFound>> GetFunctionalityById(int id, AppDbContext db)
    {
        var user = await db.Functionalities.FindAsync(id);
        return user is not null
            ? TypedResults.Ok(FunctionalityResponseDto.FromEntity(user))
            : TypedResults.NotFound();
    }

    static async Task<Results<Created<FunctionalityResponseDto>, BadRequest>> CreateFunctionality(FunctionalityDto dto, AppDbContext db)
    {
        try
        {
            Functionality newFunctionality = dto.ToEntity();

            await db.Functionalities.AddAsync(newFunctionality);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/functionalities/{newFunctionality.Id}", FunctionalityResponseDto.FromEntity(newFunctionality));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return TypedResults.BadRequest();
        }
    }

    static async Task<Results<NoContent, NotFound>> UpdateFunctionality(int id, FunctionalityDto dto, AppDbContext db)
    {
        var dbFunctionality = await db.Functionalities.FindAsync(id);
        if (dbFunctionality is null)
            return TypedResults.NotFound();

        Functionality updatedFunctionality = dto.ToEntity();

        updatedFunctionality.Id = id;

        dto.ApplyToEntity(dbFunctionality);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound>> PartialUpdateFunctionality(int id, PartialUpdateFunctionalityDto dto, AppDbContext db)
    {
        var dbFunctionality = await db.Functionalities.FindAsync(id);
        if (dbFunctionality is null)
            return TypedResults.NotFound();

        dto.ApplyToEntity(dbFunctionality);

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound>> DeleteFunctionality(int id, AppDbContext db)
    {
        var user = await db.Functionalities.FindAsync(id);
        if (user is null)
            return TypedResults.NotFound();

        db.Functionalities.Remove(user);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
