using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PorquinhoApi.Data;
using PorquinhoApi.IntegrationTests.Infrastructure;
using PorquinhoApi.Models;
using Xunit;

namespace PorquinhoApi.IntegrationTests.Endpoints;

public class SubscriptionStatusesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SubscriptionStatusesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllSubscriptionStatuses_WhenStatusesExist_ShouldReturnOk()
    {
        // Arrange
        var activeDescription = $"Ativo {Guid.NewGuid():N}";
        var cancelledDescription = $"Cancelada {Guid.NewGuid():N}";

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.SubscriptionStatuses.RemoveRange(db.SubscriptionStatuses);
            await db.SaveChangesAsync();

            db.SubscriptionStatuses.AddRange(
                new SubscriptionStatus
                {
                    Description = activeDescription,
                    Code = $"ACTIVE_{Guid.NewGuid():N}".Substring(0, 20)
                },
                new SubscriptionStatus
                {
                    Description = cancelledDescription,
                    Code = $"CANCEL_{Guid.NewGuid():N}".Substring(0, 20)
                }
            );

            await db.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/subscription-statuses?page=1&pageSize=10");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain(activeDescription);
        content.Should().Contain(cancelledDescription);
    }

    [Fact]
    public async Task GetSubscriptionStatusById_WhenStatusExists_ShouldReturnOk()
    {
        int statusId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SubscriptionStatuses.RemoveRange(db.SubscriptionStatuses);
            await db.SaveChangesAsync();

            var status = new SubscriptionStatus
            {
                Description = "Em teste",
                Code = "TEST"
            };

            db.SubscriptionStatuses.Add(status);
            await db.SaveChangesAsync();
            statusId = status.Id;
        }

        var response = await _client.GetAsync($"/subscription-statuses/{statusId}");
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Em teste");
        content.Should().Contain("TEST");
    }

    [Fact]
    public async Task GetSubscriptionStatusById_WhenStatusDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync("/subscription-statuses/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSubscriptionStatus_WhenPayloadIsValid_ShouldReturnCreated()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SubscriptionStatuses.RemoveRange(db.SubscriptionStatuses);
            await db.SaveChangesAsync();
        }

        var request = new
        {
            description = "Pendente",
            code = "PENDING"
        };

        var response = await _client.PostAsJsonAsync("/subscription-statuses", request);
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Created, content);
        content.Should().Contain("Pendente");
        content.Should().Contain("PENDING");
    }

    [Fact]
    public async Task CreateSubscriptionStatus_WhenPayloadIsInvalid_ShouldReturnBadRequest()
    {
        var request = new
        {
            description = "",
            code = ""
        };

        var response = await _client.PostAsJsonAsync("/subscription-statuses", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateSubscriptionStatus_WhenStatusExists_ShouldReturnOk()
    {
        int statusId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SubscriptionStatuses.RemoveRange(db.SubscriptionStatuses);
            await db.SaveChangesAsync();

            var status = new SubscriptionStatus
            {
                Description = "Original",
                Code = "ORIG"
            };

            db.SubscriptionStatuses.Add(status);
            await db.SaveChangesAsync();
            statusId = status.Id;
        }

        var request = new
        {
            description = "Atualizada",
            code = "UPDATED"
        };

        var response = await _client.PutAsJsonAsync($"/subscription-statuses/{statusId}", request);
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Atualizada");
        content.Should().Contain("UPDATED");
    }

    [Fact]
    public async Task UpdateSubscriptionStatus_WhenStatusDoesNotExist_ShouldReturnNotFound()
    {
        var request = new
        {
            description = "Atualizada",
            code = "UPDATED"
        };

        var response = await _client.PutAsJsonAsync("/subscription-statuses/999999", request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PartialUpdateSubscriptionStatus_WhenStatusExists_ShouldReturnOk()
    {
        int statusId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SubscriptionStatuses.RemoveRange(db.SubscriptionStatuses);
            await db.SaveChangesAsync();

            var status = new SubscriptionStatus
            {
                Description = "Original",
                Code = "ORIG"
            };

            db.SubscriptionStatuses.Add(status);
            await db.SaveChangesAsync();
            statusId = status.Id;
        }

        var request = new
        {
            description = "Alterada"
        };

        var response = await _client.PatchAsJsonAsync($"/subscription-statuses/{statusId}", request);
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Alterada");
    }

    [Fact]
    public async Task PartialUpdateSubscriptionStatus_WhenStatusDoesNotExist_ShouldReturnNotFound()
    {
        var request = new
        {
            description = "Alterada"
        };

        var response = await _client.PatchAsJsonAsync("/subscription-statuses/999999", request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSubscriptionStatus_WhenStatusExists_ShouldReturnNoContent()
    {
        int statusId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SubscriptionStatuses.RemoveRange(db.SubscriptionStatuses);
            await db.SaveChangesAsync();

            var status = new SubscriptionStatus
            {
                Description = "Excluir",
                Code = "DEL"
            };

            db.SubscriptionStatuses.Add(status);
            await db.SaveChangesAsync();
            statusId = status.Id;
        }

        var deleteResponse = await _client.DeleteAsync($"/subscription-statuses/{statusId}");
        var getResponse = await _client.GetAsync($"/subscription-statuses/{statusId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSubscriptionStatus_WhenStatusDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.DeleteAsync("/subscription-statuses/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchSubscriptionStatuses_WhenQueryMatches_ShouldReturnFilteredResults()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SubscriptionStatuses.RemoveRange(db.SubscriptionStatuses);
            await db.SaveChangesAsync();

            db.SubscriptionStatuses.AddRange(
                new SubscriptionStatus { Description = "Status Ativo", Code = "ACTIVE" },
                new SubscriptionStatus { Description = "Status Cancelado", Code = "CANCELLED" }
            );

            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync("/subscription-statuses/search?q=Ativo");
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Status Ativo");
        content.Should().NotContain("Status Cancelado");
    }

    [Fact]
    public async Task SearchSubscriptionStatuses_WhenQueryIsEmpty_ShouldReturnBadRequest()
    {
        var response = await _client.GetAsync("/subscription-statuses/search?q=");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}