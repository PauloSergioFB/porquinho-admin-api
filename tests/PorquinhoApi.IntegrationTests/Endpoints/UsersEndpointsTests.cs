using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PorquinhoApi.Data;
using PorquinhoApi.IntegrationTests.Infrastructure;
using PorquinhoApi.Models;
using Xunit;

namespace PorquinhoApi.IntegrationTests.Endpoints;

public class UsersEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UsersEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUserById_WhenUserExists_ShouldReturnOk()
    {
        // Arrange
        int userId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();

            var user = new User
            {
                FullName = "Paulo França",
                Email = $"paulo_{Guid.NewGuid():N}@email.com",
                HashedPassword = "12345678"
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            userId = user.Id;
        }

        // Act
        var response = await _client.GetAsync($"/users/{userId}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Paulo França");
    }

    [Fact]
    public async Task GetUserById_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/users/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateUser_WhenPayloadIsValid_ShouldReturnCreated()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();
        }

        var uniqueEmail = $"helena_{Guid.NewGuid():N}@email.com";

        var request = new
        {
            full_name = "Helena Souza",
            email = uniqueEmail,
            password = "12345678"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/users", request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);
        content.Should().Contain("Helena Souza");
        content.Should().Contain(uniqueEmail);
    }

    [Fact]
    public async Task CreateUser_WhenPayloadIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            full_name = "",
            email = "",
            password = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUser_WhenUserExists_ShouldReturnOk()
    {
        // Arrange
        int userId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();

            var user = new User
            {
                FullName = "Nome Antigo",
                Email = $"old_{Guid.NewGuid():N}@email.com",
                HashedPassword = "12345678"
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();
            userId = user.Id;
        }

        var request = new
        {
            full_name = "Nome Novo",
            email = $"new_{Guid.NewGuid():N}@email.com",
            password = "87654321"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/users/{userId}", request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Nome Novo");
    }

    [Fact]
    public async Task UpdateUser_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var request = new
        {
            full_name = "Nome Novo",
            email = $"notfound_{Guid.NewGuid():N}@email.com",
            password = "12345678"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/users/999999", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PartialUpdateUser_WhenUserExists_ShouldReturnOk()
    {
        // Arrange  
        int userId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();

            var user = new User
            {
                FullName = "Nome Original",
                Email = $"patch_{Guid.NewGuid():N}@email.com",
                HashedPassword = "12345678"
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();
            userId = user.Id;
        }

        var request = new
        {
            full_name = "Nome Alterado"
        };

        // Act
        var response = await _client.PatchAsJsonAsync($"/users/{userId}", request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Nome Alterado");
    }

    [Fact]
    public async Task DeleteUser_WhenUserExists_ShouldReturnNoContent()
    {
        // Arrange
        var user = new User
        {
            FullName = "Usuário para excluir",
            Email = "delete@email.com"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        // Act
        var deleteResponse = await _client.DeleteAsync($"/users/{user.Id}");
        var getResponse = await _client.GetAsync($"/users/{user.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchUsers_WhenQueryMatches_ShouldReturnFilteredUsers()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();

            db.Users.AddRange(
                new User
                {
                    FullName = "Maria Silva",
                    Email = $"maria_{Guid.NewGuid():N}@email.com",
                    HashedPassword = "12345678"
                },
                new User
                {
                    FullName = "João Pedro",
                    Email = $"joao_{Guid.NewGuid():N}@email.com",
                    HashedPassword = "12345678"
                }
            );

            await db.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/users/search?q=Maria");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Maria Silva");
        content.Should().NotContain("João Pedro");
    }
}