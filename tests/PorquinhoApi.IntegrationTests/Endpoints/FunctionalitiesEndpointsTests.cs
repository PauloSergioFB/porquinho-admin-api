using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PorquinhoApi.Data;
using PorquinhoApi.IntegrationTests.Infrastructure;
using PorquinhoApi.Models;
using Xunit;

namespace PorquinhoApi.IntegrationTests.Endpoints;

public class FunctionalitiesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public FunctionalitiesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllFunctionalities_WhenFunctionalitiesExist_ShouldReturnOk()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Functionalities.RemoveRange(db.Functionalities);
            await db.SaveChangesAsync();

            db.Functionalities.AddRange(
                new Functionality { Name = "Dashboard" },
                new Functionality { Name = "Relatórios" }
            );

            await db.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/functionalities?page=1&pageSize=10");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Dashboard");
        content.Should().Contain("Relatórios");
    }

    [Fact]
    public async Task GetFunctionalityById_WhenFunctionalityExists_ShouldReturnOk()
    {
        // Arrange
        int functionalityId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Functionalities.RemoveRange(db.Functionalities);
            await db.SaveChangesAsync();

            var functionality = new Functionality
            {
                Name = "Exportação"
            };

            db.Functionalities.Add(functionality);
            await db.SaveChangesAsync();

            functionalityId = functionality.Id;
        }

        // Act
        var response = await _client.GetAsync($"/functionalities/{functionalityId}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Exportação");
    }

    [Fact]
    public async Task GetFunctionalityById_WhenFunctionalityDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/functionalities/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateFunctionality_WhenPayloadIsValid_ShouldReturnCreated()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Functionalities.RemoveRange(db.Functionalities);
            await db.SaveChangesAsync();
        }

        var request = new
        {
            name = "Gestão de usuários",
            code = "USR_MGMT"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/functionalities", request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);
        content.Should().Contain("Gestão de usuários");
        content.Should().Contain("USR_MGMT");
    }

    [Fact]
    public async Task CreateFunctionality_WhenPayloadIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Functionalities.RemoveRange(db.Functionalities);
            await db.SaveChangesAsync();
        }

        var request = new
        {
            name = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/functionalities", request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, content);
    }

    [Fact]
    public async Task UpdateFunctionality_WhenFunctionalityExists_ShouldReturnOk()
    {
        // Arrange
        int functionalityId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Functionalities.RemoveRange(db.Functionalities);
            await db.SaveChangesAsync();

            var functionality = new Functionality
            {
                Name = "Nome antigo",
                Code = "OLD_CODE"
            };

            db.Functionalities.Add(functionality);
            await db.SaveChangesAsync();

            functionalityId = functionality.Id;
        }

        var request = new
        {
            name = "Nome novo",
            code = "NEW_CODE"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/functionalities/{functionalityId}", request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Nome novo");
        content.Should().Contain("NEW_CODE");
    }

    [Fact]
    public async Task UpdateFunctionality_WhenFunctionalityDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var request = new
        {
            name = "Nome novo",
            code = "NEW_CODE"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/functionalities/999999", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PartialUpdateFunctionality_WhenFunctionalityExists_ShouldReturnOk()
    {
        // Arrange
        int functionalityId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Functionalities.RemoveRange(db.Functionalities);
            await db.SaveChangesAsync();

            var functionality = new Functionality
            {
                Name = "Nome original"
            };

            db.Functionalities.Add(functionality);
            await db.SaveChangesAsync();

            functionalityId = functionality.Id;
        }

        var request = new
        {
            name = "Nome alterado"
        };

        // Act
        var response = await _client.PatchAsJsonAsync($"/functionalities/{functionalityId}", request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Nome alterado");
    }

    [Fact]
    public async Task PartialUpdateFunctionality_WhenFunctionalityDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var request = new
        {
            name = "Nome alterado"
        };

        // Act
        var response = await _client.PatchAsJsonAsync("/functionalities/999999", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteFunctionality_WhenFunctionalityExists_ShouldReturnNoContent()
    {
        // Arrange
        int functionalityId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Functionalities.RemoveRange(db.Functionalities);
            await db.SaveChangesAsync();

            var functionality = new Functionality
            {
                Name = "Para excluir"
            };

            db.Functionalities.Add(functionality);
            await db.SaveChangesAsync();

            functionalityId = functionality.Id;
        }

        // Act
        var deleteResponse = await _client.DeleteAsync($"/functionalities/{functionalityId}");
        var getResponse = await _client.GetAsync($"/functionalities/{functionalityId}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteFunctionality_WhenFunctionalityDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/functionalities/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchFunctionalities_WhenQueryMatches_ShouldReturnFilteredResults()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Functionalities.RemoveRange(db.Functionalities);
            await db.SaveChangesAsync();

            db.Functionalities.AddRange(
                new Functionality { Name = "Dashboard Financeiro" },
                new Functionality { Name = "Controle de Assinaturas" }
            );

            await db.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/functionalities/search?q=Dashboard");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Dashboard Financeiro");
        content.Should().NotContain("Controle de Assinaturas");
    }

    [Fact]
    public async Task SearchFunctionalities_WhenQueryIsEmpty_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/functionalities/search?q=");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}