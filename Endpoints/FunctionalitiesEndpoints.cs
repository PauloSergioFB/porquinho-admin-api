using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PorquinhoApi.Data;
using PorquinhoApi.DTOs.Functionalities;
using PorquinhoApi.Filters;
using PorquinhoApi.Models;
using PorquinhoApi.Services;
using PorquinhoApi.Utils;

namespace PorquinhoApi.Endpoints;

public static class FunctionalitiesEndpoints
{
    public static void MapFunctionalitiesEndpoints(this WebApplication app)
    {
        var functionalities = app.MapGroup("/functionalities").WithTags("Functionalities");

        functionalities.MapGet("/", GetAllFunctionalities)
            .WithName("GetAllFunctionalities")
            .WithSummary("Retorna todos as funcionalidades cadastradas")
            .WithDescription("Obtém uma lista completa de funcionalidades do sistema")
            .Produces<Ok<PagedResponse<FunctionalityResponseDto>>>(StatusCodes.Status200OK);

        functionalities.MapGet("/{id:int}", GetFunctionalityById)
            .WithName("GetFunctionalityById")
            .WithSummary("Retorna uma funcionalidade específica")
            .WithDescription("Obtém uma funcionalidade pelo ID informado")
            .Produces<Ok<FunctionalityResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        functionalities.MapPost("/", CreateFunctionality)
            .WithSummary("Cria uma nova funcionalidade")
            .WithName("CreateFunctionality")
            .WithDescription("Adiciona uma nova funcionalidade ao banco de dados")
            .AddEndpointFilter<ValidationFilter<FunctionalityDto>>()
            .Produces<Created<FunctionalityResponseDto>>(StatusCodes.Status201Created)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);

        functionalities.MapPut("/{id:int}", UpdateFunctionality)
            .WithName("UpdateFunctionality")
            .WithSummary("Atualiza todos os dados de uma funcionalidade")
            .WithDescription("Substitui todos os campos de uma funcionalidade existente pelo ID")
            .AddEndpointFilter<ValidationFilter<FunctionalityDto>>()
            .Produces<Ok<FunctionalityResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        functionalities.MapPatch("/{id:int}", PartialUpdateFunctionality)
            .WithName("PartialUpdateFunctionality")
            .WithSummary("Atualiza parcialmente uma funcionalidade")
            .WithDescription("Modifica apenas os campos enviados no corpo da requisição")
            .AddEndpointFilter<ValidationFilter<PartialUpdateFunctionalityDto>>()
            .Produces<Ok<FunctionalityResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        functionalities.MapDelete("/{id:int}", DeleteFunctionality)
            .WithName("DeleteFunctionality")
            .WithSummary("Remove uma funcionalidade")
            .WithDescription("Exclui uma funcionalidade do sistema pelo ID informado")
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        functionalities.MapGet("/search", SearchFunctionalities)
            .WithName("SearchFunctionalities")
            .WithSummary("Busca funcionalidades pelo nome")
            .WithDescription("Realiza uma busca textual nas funcionalidades cadastradas.")
            .Produces<Ok<PagedResponse<FunctionalityResponseDto>>>(StatusCodes.Status200OK)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);
    }

    static async Task<Ok<PagedResponse<FunctionalityResponseDto>>> GetAllFunctionalities(
        AppDbContext db,
        IHateoasLinkService hateoas,
        HttpContext http,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var totalFunctionalities = await db.Functionalities.CountAsync();

        var functionalities = await db.Functionalities
            .OrderBy(f => f.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = functionalities.Select(FunctionalityResponseDto.FromEntity).ToList();

        foreach (var dto in dtos)
        {
            hateoas.AddItemLinks(
                dto,
                routeNameGetById: "GetFunctionalityById",
                routeValuesForItem: new { id = dto.Id },
                extras:
                [
                    ("get_all_functionalities", "GET", "GetAllFunctionalities", null),
                    ("create_functionality", "POST", "CreateFunctionality", null),
                    ("update_functionality", "PUT", "UpdateFunctionality", null),
                    ("partial_update_functionality", "PATCH", "PartialUpdateFunctionality", null),
                    ("delete_functionality", "DELETE", "DeleteFunctionality", null)
                ]
            );
        }

        var response = PaginationResponseBuilder.Build(
            items: dtos,
            page: page,
            pageSize: pageSize,
            totalItems: totalFunctionalities,
            routeBase: "functionalities",
            http: http
        );

        return TypedResults.Ok(response);
    }

    static async Task<Results<Ok<FunctionalityResponseDto>, NotFound>> GetFunctionalityById(int id,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var user = await db.Functionalities.FindAsync(id);
        if (user is null) return TypedResults.NotFound();

        var dto = FunctionalityResponseDto.FromEntity(user);
        hateoas.AddItemLinks(
            dto,
            routeNameGetById: "GetFunctionalityById",
            routeValuesForItem: new { id = dto.Id },
            extras:
            [
                ("get_all_functionalities", "GET", "GetAllFunctionalities", null),
                ("create_functionality", "POST", "CreateFunctionality", null),
                ("update_functionality", "PUT", "UpdateFunctionality", null),
                ("partial_update_functionality", "PATCH", "PartialUpdateFunctionality", null),
                ("delete_functionality", "DELETE", "DeleteFunctionality", null)
            ]
            );

        return TypedResults.Ok(dto);
    }

    static async Task<Results<Created<FunctionalityResponseDto>, BadRequest>> CreateFunctionality(
        FunctionalityDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        try
        {
            Functionality newFunctionality = dto.ToEntity();

            await db.Functionalities.AddAsync(newFunctionality);
            await db.SaveChangesAsync();

            var dtoResponse = FunctionalityResponseDto.FromEntity(newFunctionality);
            hateoas.AddItemLinks(
                dtoResponse,
                routeNameGetById: "GetFunctionalityById",
                routeValuesForItem: new { id = dtoResponse.Id },
                extras:
                [
                    ("get_all_functionalities", "GET", "GetAllFunctionalities", null),
                    ("update_functionality", "PUT", "UpdateFunctionality", null),
                    ("partial_update_functionality", "PATCH", "PartialUpdateFunctionality", null),
                    ("delete_functionality", "DELETE", "DeleteFunctionality", null)
                ]
            );

            return TypedResults.Created(
                $"/functionalities/{newFunctionality.Id}",
                dtoResponse
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return TypedResults.BadRequest();
        }
    }

    static async Task<Results<Ok<FunctionalityResponseDto>, NotFound>> UpdateFunctionality(int id,
        FunctionalityDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var dbFunctionality = await db.Functionalities.FindAsync(id);
        if (dbFunctionality is null)
            return TypedResults.NotFound();

        dto.ApplyToEntity(dbFunctionality);
        await db.SaveChangesAsync();

        var dtoResponse = FunctionalityResponseDto.FromEntity(dbFunctionality);
        hateoas.AddItemLinks(
            dtoResponse,
            routeNameGetById: "GetFunctionalityById",
            routeValuesForItem: new { id = dtoResponse.Id },
            extras:
            [
                ("get_all_functionalities", "GET", "GetAllFunctionalities", null),
                ("create_functionality", "POST", "CreateFunctionality", null),
                ("partial_update_functionality", "PATCH", "PartialUpdateFunctionality", null),
                ("delete_functionality", "DELETE", "DeleteFunctionality", null)
            ]
        );

        return TypedResults.Ok(dtoResponse);
    }

    static async Task<Results<Ok<FunctionalityResponseDto>, NotFound>> PartialUpdateFunctionality(
        int id,
        PartialUpdateFunctionalityDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var dbFunctionality = await db.Functionalities.FindAsync(id);
        if (dbFunctionality is null)
            return TypedResults.NotFound();

        dto.ApplyToEntity(dbFunctionality);
        await db.SaveChangesAsync();

        var dtoResponse = FunctionalityResponseDto.FromEntity(dbFunctionality);
        hateoas.AddItemLinks(
            dtoResponse,
            routeNameGetById: "GetFunctionalityById",
            routeValuesForItem: new { id = dtoResponse.Id },
            extras:
            [
                ("get_all_functionalities", "GET", "GetAllFunctionalities", null),
                ("create_functionality", "POST", "CreateFunctionality", null),
                ("update_functionality", "PUT", "UpdateFunctionality", null),
                ("delete_functionality", "DELETE", "DeleteFunctionality", null)
            ]
        );

        return TypedResults.Ok(dtoResponse);
    }

    static async Task<Results<NoContent, NotFound>> DeleteFunctionality(int id, AppDbContext db, IHateoasLinkService hateoas)
    {
        var user = await db.Functionalities.FindAsync(id);
        if (user is null)
            return TypedResults.NotFound();

        db.Functionalities.Remove(user);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<Ok<PagedResponse<FunctionalityResponseDto>>, BadRequest>> SearchFunctionalities(
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

        var query = db.Functionalities
            .Where(f => EF.Functions.Like(f.Name, $"%{q}%"))
            .OrderBy(f => f.Id);

        var totalFunctionalities = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalFunctionalities / (double)pageSize);

        var functionalities = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = functionalities.Select(FunctionalityResponseDto.FromEntity).ToList();

        foreach (var dto in dtos)
        {
            hateoas.AddItemLinks(
                dto,
                routeNameGetById: "GetFunctionalityById",
                routeValuesForItem: new { id = dto.Id },
                extras:
                [
                    ("get_all_functionalities", "GET", "GetAllFunctionalities", null),
                    ("create_functionality", "POST", "CreateFunctionality", null),
                    ("update_functionality", "PUT", "UpdateFunctionality", null),
                    ("delete_functionality", "DELETE", "DeleteFunctionality", null)
                ]
            );
        }

        var response = PaginationResponseBuilder.Build(
            items: dtos,
            page: page,
            pageSize: pageSize,
            totalItems: totalFunctionalities,
            routeBase: $"functionalities/search?q={q}",
            http: http
        );

        return TypedResults.Ok(response);
    }
}
