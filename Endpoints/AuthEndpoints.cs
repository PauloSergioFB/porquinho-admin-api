using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using PorquinhoApi.Data;
using PorquinhoApi.Models;

namespace PorquinhoApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("/auth")
            .WithTags("Authentication");

        auth.MapPost("/login", Login)
            .WithName("Login")
            .WithSummary("Autentica um usuário")
            .WithDescription("""
                Realiza a autenticação de um usuário utilizando email e senha.
                Retorna um token JWT válido para acesso aos endpoints protegidos da API.
                """)
            .Produces<Ok<AuthResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Ok<AuthResponseDto>, UnauthorizedHttpResult>> Login(
        LoginRequestDto request,
        AppDbContext db,
        IPasswordHasher<User> passwordHasher,
        JwtTokenService jwtTokenService)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null)
            return TypedResults.Unauthorized();

        var result = passwordHasher.VerifyHashedPassword(
            user,
            user.HashedPassword,
            request.Password
        );

        if (result == PasswordVerificationResult.Failed)
            return TypedResults.Unauthorized();

        var token = jwtTokenService.GenerateToken(user);

        return TypedResults.Ok(new AuthResponseDto(token));
    }
}
