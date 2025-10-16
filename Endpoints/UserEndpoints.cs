using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PorquinhoApi.Data;
using PorquinhoApi.DTOs.Users;
using PorquinhoApi.Filters;
using PorquinhoApi.Models;

namespace PorquinhoApi.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var users = app.MapGroup("/users").WithTags("Users");

        users.MapGet("/", GetAllUsers)
            .WithSummary("Retorna todos os usuários cadastrados")
            .WithDescription("Obtém uma lista completa de usuários do sistema")
            .Produces<Ok<List<UserResponseDto>>>(StatusCodes.Status200OK);

        users.MapGet("/{id:int}", GetUserById)
            .WithSummary("Retorna um usuário específico")
            .WithDescription("Obtém um usuário pelo ID informado")
            .Produces<Ok<UserResponseDto>>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        users.MapPost("/", CreateUser)
            .WithSummary("Cria um novo usuário")
            .WithDescription("Adiciona um novo usuário ao banco de dados")
            .AddEndpointFilter<ValidationFilter<UserDto>>()
            .Produces<Created<UserResponseDto>>(StatusCodes.Status201Created)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);

        users.MapPut("/{id:int}", UpdateUser)
            .WithSummary("Atualiza todos os dados de um usuário")
            .WithDescription("Substitui todos os campos de um usuário existente pelo ID")
            .AddEndpointFilter<ValidationFilter<UserDto>>()
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        users.MapPatch("/{id:int}", PartialUpdateUser)
            .WithSummary("Atualiza parcialmente um usuário")
            .WithDescription("Modifica apenas os campos enviados no corpo da requisição")
            .AddEndpointFilter<ValidationFilter<PartialUpdateUserDto>>()
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);

        users.MapDelete("/{id:int}", DeleteUser)
            .WithSummary("Remove um usuário")
            .WithDescription("Exclui um usuário do sistema pelo ID informado")
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<NotFound>(StatusCodes.Status404NotFound);
    }

    static async Task<Ok<List<UserResponseDto>>> GetAllUsers(AppDbContext db)
    {
        var users = await db.Users.ToListAsync();
        return TypedResults.Ok(users.Select(UserResponseDto.FromEntity).ToList());
    }

    static async Task<Results<Ok<UserResponseDto>, NotFound>> GetUserById(int id, AppDbContext db)
    {
        var user = await db.Users.FindAsync(id);
        return user is not null
            ? TypedResults.Ok(UserResponseDto.FromEntity(user))
            : TypedResults.NotFound();
    }

    static async Task<Results<Created<UserResponseDto>, BadRequest>> CreateUser(UserDto dto, AppDbContext db)
    {
        try
        {
            User newUser = dto.ToEntity();
            newUser.CreatedAt = DateTime.UtcNow;

            await db.Users.AddAsync(newUser);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/users/{newUser.Id}", UserResponseDto.FromEntity(newUser));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return TypedResults.BadRequest();
        }
    }

    static async Task<Results<NoContent, NotFound>> UpdateUser(int id, UserDto dto, AppDbContext db)
    {
        var dbUser = await db.Users.FindAsync(id);
        if (dbUser is null)
            return TypedResults.NotFound();

        User updatedUser = dto.ToEntity();

        updatedUser.Id = id;
        updatedUser.UpdatedAt = DateTime.UtcNow;

        dto.ApplyToEntity(dbUser);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound>> PartialUpdateUser(int id, PartialUpdateUserDto dto, AppDbContext db)
    {
        var dbUser = await db.Users.FindAsync(id);
        if (dbUser is null)
            return TypedResults.NotFound();

        dto.ApplyToEntity(dbUser);

        dbUser.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound>> DeleteUser(int id, AppDbContext db)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null)
            return TypedResults.NotFound();

        db.Users.Remove(user);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
