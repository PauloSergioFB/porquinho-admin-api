using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PorquinhoApi.Data;
using PorquinhoApi.DTOs.Users;
using PorquinhoApi.Filters;
using PorquinhoApi.Models;
using PorquinhoApi.Services;
using PorquinhoApi.Utils;

namespace PorquinhoApi.Endpoints;

public static class UsersEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var users = app.MapGroup("/users").WithTags("Users");

        users.MapGet("/", GetAllUsers)
            .WithName("GetAllUsers")
            .WithSummary("Retorna todos os usuários cadastrados")
            .WithDescription("Obtém uma lista completa de usuários do sistema")
            .Produces<Ok<PagedResponse<UserResponseDto>>>(StatusCodes.Status200OK);

        users.MapGet("/{id:int}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Retorna um usuário específico")
            .WithDescription("Obtém um usuário pelo ID informado")
            .Produces<Ok<UserResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        users.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Cria um novo usuário")
            .WithDescription("Adiciona um novo usuário ao banco de dados")
            .AddEndpointFilter<ValidationFilter<UserDto>>()
            .Produces<Created<UserResponseDto>>(StatusCodes.Status201Created)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);

        users.MapPut("/{id:int}", UpdateUser)
            .WithName("UpdateUser")
            .WithSummary("Atualiza todos os dados de um usuário")
            .WithDescription("Substitui todos os campos de um usuário existente pelo ID")
            .AddEndpointFilter<ValidationFilter<UserDto>>()
            .Produces<Ok<UserResponseDto>>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        users.MapPatch("/{id:int}", PartialUpdateUser)
            .WithName("PartialUpdateUser")
            .WithSummary("Atualiza parcialmente um usuário")
            .WithDescription("Modifica apenas os campos enviados no corpo da requisição")
            .AddEndpointFilter<ValidationFilter<PartialUpdateUserDto>>()
            .Produces<Ok<UserResponseDto>>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        users.MapDelete("/{id:int}", DeleteUser)
            .WithName("DeleteUser")
            .WithSummary("Remove um usuário")
            .WithDescription("Exclui um usuário do sistema pelo ID informado")
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        users.MapGet("/search", SearchUsers)
            .WithName("SearchFunctionalities")
            .WithSummary("Busca usuários pelo nome")
            .WithDescription("Realiza uma busca textual nos usuários cadastrados.")
            .Produces<Ok<PagedResponse<UserResponseDto>>>(StatusCodes.Status200OK)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);
    }

    static async Task<Ok<PagedResponse<UserResponseDto>>> GetAllUsers(
        AppDbContext db,
        IHateoasLinkService hateoas,
        HttpContext http,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var totalUsers = await db.Users.CountAsync();

        var users = await db.Users
            .OrderBy(f => f.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = users.Select(UserResponseDto.FromEntity).ToList();

        foreach (var dto in dtos)
        {
            hateoas.AddItemLinks(
                dto,
                routeNameGetById: "GetUserById",
                routeValuesForItem: new { id = dto.Id },
                extras:
                [
                    ("get_all_users", "GET", "GetAllUsers", null),
                    ("create_user", "POST", "CreateUser", null),
                    ("update_user", "PUT", "UpdateUser", null),
                    ("partial_update_user", "PATCH", "PartialUpdateUser", null),
                    ("delete_user", "DELETE", "DeleteUser", null)
                ]
            );
        }

        var response = PaginationResponseBuilder.Build(
            items: dtos,
            page: page,
            pageSize: pageSize,
            totalItems: totalUsers,
            routeBase: "users",
            http: http
        );

        return TypedResults.Ok(response);
    }

    static async Task<Results<Ok<UserResponseDto>, NotFound>> GetUserById(int id,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return TypedResults.NotFound();

        var dto = UserResponseDto.FromEntity(user);
        hateoas.AddItemLinks(
            dto,
            routeNameGetById: "GetUserById",
            routeValuesForItem: new { id = dto.Id },
            extras:
            [
                ("get_all_users", "GET", "GetAllUsers", null),
                ("create_user", "POST", "CreateUser", null),
                ("update_user", "PUT", "UpdateUser", null),
                ("partial_update_user", "PATCH", "PartialUpdateUser", null),
                ("delete_user", "DELETE", "DeleteUser", null)
            ]
            );

        return TypedResults.Ok(dto);
    }

    static async Task<Results<Created<UserResponseDto>, BadRequest>> CreateUser(
        UserDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        try
        {
            User newUser = dto.ToEntity();

            await db.Users.AddAsync(newUser);
            await db.SaveChangesAsync();

            var dtoResponse = UserResponseDto.FromEntity(newUser);
            hateoas.AddItemLinks(
                dtoResponse,
                routeNameGetById: "GetUserById",
                routeValuesForItem: new { id = dtoResponse.Id },
                extras:
                [
                    ("get_all_users", "GET", "GetAllUsers", null),
                    ("update_user", "PUT", "UpdateUser", null),
                    ("partial_update_user", "PATCH", "PartialUpdateUser", null),
                    ("delete_user", "DELETE", "DeleteUser", null)
                ]
            );

            return TypedResults.Created(
                $"/users/{newUser.Id}",
                dtoResponse
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return TypedResults.BadRequest();
        }
    }

    static async Task<Results<Ok<UserResponseDto>, NotFound>> UpdateUser(int id,
        UserDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var dbUser = await db.Users.FindAsync(id);
        if (dbUser is null)
            return TypedResults.NotFound();

        dto.ApplyToEntity(dbUser);
        await db.SaveChangesAsync();

        var dtoResponse = UserResponseDto.FromEntity(dbUser);
        hateoas.AddItemLinks(
            dtoResponse,
            routeNameGetById: "GetUserById",
            routeValuesForItem: new { id = dtoResponse.Id },
            extras:
            [
                ("get_all_users", "GET", "GetAllUsers", null),
                ("create_user", "POST", "CreateUser", null),
                ("partial_update_user", "PATCH", "PartialUpdateUser", null),
                ("delete_user", "DELETE", "DeleteUser", null)
            ]
        );

        return TypedResults.Ok(dtoResponse);
    }

    static async Task<Results<Ok<UserResponseDto>, NotFound>> PartialUpdateUser(
        int id,
        PartialUpdateUserDto dto,
        AppDbContext db,
        IHateoasLinkService hateoas)
    {
        var dbUser = await db.Users.FindAsync(id);
        if (dbUser is null)
            return TypedResults.NotFound();

        dto.ApplyToEntity(dbUser);
        await db.SaveChangesAsync();

        var dtoResponse = UserResponseDto.FromEntity(dbUser);
        hateoas.AddItemLinks(
            dtoResponse,
            routeNameGetById: "GetUserById",
            routeValuesForItem: new { id = dtoResponse.Id },
            extras:
            [
                ("get_all_users", "GET", "GetAllUsers", null),
                ("create_user", "POST", "CreateUser", null),
                ("update_user", "PUT", "UpdateUser", null),
                ("delete_user", "DELETE", "DeleteUser", null)
            ]
        );

        return TypedResults.Ok(dtoResponse);
    }

    static async Task<Results<NoContent, NotFound>> DeleteUser(int id, AppDbContext db, IHateoasLinkService hateoas)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null)
            return TypedResults.NotFound();

        db.Users.Remove(user);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<Ok<PagedResponse<UserResponseDto>>, BadRequest>> SearchUsers(
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
        var query = db.Users
            .Where(f => f.Id == idValue)
            .OrderBy(f => f.Id);

        var totalUsers = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = users.Select(UserResponseDto.FromEntity).ToList();

        foreach (var dto in dtos)
        {
            hateoas.AddItemLinks(
                dto,
                routeNameGetById: "GetUserById",
                routeValuesForItem: new { id = dto.Id },
                extras:
                [
                    ("get_all_users", "GET", "GetAllUsers", null),
                    ("create_user", "POST", "CreateUser", null),
                    ("update_user", "PUT", "UpdateUser", null),
                    ("delete_user", "DELETE", "DeleteUser", null)
                ]
            );
        }

        var response = PaginationResponseBuilder.Build(
            items: dtos,
            page: page,
            pageSize: pageSize,
            totalItems: totalUsers,
            routeBase: $"users/search?q={q}",
            http: http
        );

        return TypedResults.Ok(response);
    }
}
