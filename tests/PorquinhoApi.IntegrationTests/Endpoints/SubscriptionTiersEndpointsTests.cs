using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PorquinhoApi.Data;
using PorquinhoApi.IntegrationTests.Infrastructure;
using PorquinhoApi.Models;
using Xunit;

namespace PorquinhoApi.IntegrationTests.Endpoints;

public class SubscriptionTiersEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SubscriptionTiersEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllSubscriptionTiers_WhenTiersExist_ShouldReturnOk()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SubscriptionTiers.RemoveRange(db.SubscriptionTiers);
            await db.SaveChangesAsync();

            db.SubscriptionTiers.AddRange(
                new SubscriptionTier { Name = "Basic", Description = "Plano básico", Price = 19.90m },
                new SubscriptionTier { Name = "Pro", Description = "Plano pro", Price = 49.90m }
            );

            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync("/subscription-tiers?page=1&pageSize=10");
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Basic");
        content.Should().Contain("Pro");
    }

    [Fact]
    public async Task GetSubscriptionTierById_WhenTierExists_ShouldReturnOk()
    {
        int tierId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SubscriptionTiers.RemoveRange(db.SubscriptionTiers);
            await db.SaveChangesAsync();

            var tier = new SubscriptionTier
            {
                Name = "Enterprise",
                Description = "Plano enterprise",
                Price = 99.90m
            };

            db.SubscriptionTiers.Add(tier);
            await db.SaveChangesAsync();
            tierId = tier.Id;
        }

        var response = await _client.GetAsync($"/subscription-tiers/{tierId}");
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Enterprise");
    }

    [Fact]
    public async Task GetSubscriptionTierById_WhenTierDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync("/subscription-tiers/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSubscriptionTier_WhenPayloadIsValid_ShouldReturnCreated()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SubscriptionTiers.RemoveRange(db.SubscriptionTiers);
            await db.SaveChangesAsync();
        }

        var request = new
        {
            name = "Gold",
            description = "Plano gold",
            price = 79.90m,
            functionality_ids = Array.Empty<int>()
        };

        var response = await _client.PostAsJsonAsync("/subscription-tiers", request);
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Created, content);
        content.Should().Contain("Gold");
    }

    [Fact]
    public async Task CreateSubscriptionTier_WhenPayloadIsInvalid_ShouldReturnBadRequest()
    {
        var request = new
        {
            name = "",
            description = "Inválido",
            price = 0m,
            functionality_ids = Array.Empty<int>()
        };

        var response = await _client.PostAsJsonAsync("/subscription-tiers", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateSubscriptionTier_WhenTierExists_ShouldReturnOk()
    {
        int tierId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SubscriptionTiers.RemoveRange(db.SubscriptionTiers);
            await db.SaveChangesAsync();

            var tier = new SubscriptionTier
            {
                Name = "Starter",
                Description = "Inicial",
                Price = 29.90m
            };

            db.SubscriptionTiers.Add(tier);
            await db.SaveChangesAsync();
            tierId = tier.Id;
        }

        var request = new
        {
            name = "Starter Plus",
            description = "Atualizado",
            price = 39.90m,
            functionality_ids = Array.Empty<int>()
        };

        var response = await _client.PutAsJsonAsync($"/subscription-tiers/{tierId}", request);
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Starter Plus");
    }

    [Fact]
    public async Task UpdateSubscriptionTier_WhenTierDoesNotExist_ShouldReturnNotFound()
    {
        var request = new
        {
            name = "Starter Plus",
            description = "Atualizado",
            price = 39.90m,
            functionality_ids = Array.Empty<int>()
        };

        var response = await _client.PutAsJsonAsync("/subscription-tiers/999999", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PartialUpdateSubscriptionTier_WhenTierExists_ShouldReturnOk()
    {
        int tierId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SubscriptionTiers.RemoveRange(db.SubscriptionTiers);
            await db.SaveChangesAsync();

            var tier = new SubscriptionTier
            {
                Name = "Silver",
                Description = "Original",
                Price = 59.90m
            };

            db.SubscriptionTiers.Add(tier);
            await db.SaveChangesAsync();
            tierId = tier.Id;
        }

        var request = new
        {
            name = "Silver Updated"
        };

        var response = await _client.PatchAsJsonAsync($"/subscription-tiers/{tierId}", request);
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Silver Updated");
    }

    [Fact]
    public async Task PartialUpdateSubscriptionTier_WhenTierDoesNotExist_ShouldReturnNotFound()
    {
        var request = new
        {
            name = "Silver Updated"
        };

        var response = await _client.PatchAsJsonAsync("/subscription-tiers/999999", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSubscriptionTier_WhenTierExists_ShouldReturnNoContent()
    {
        int tierId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SubscriptionTiers.RemoveRange(db.SubscriptionTiers);
            await db.SaveChangesAsync();

            var tier = new SubscriptionTier
            {
                Name = "Delete Tier",
                Description = "Excluir",
                Price = 10m
            };

            db.SubscriptionTiers.Add(tier);
            await db.SaveChangesAsync();
            tierId = tier.Id;
        }

        var deleteResponse = await _client.DeleteAsync($"/subscription-tiers/{tierId}");
        var getResponse = await _client.GetAsync($"/subscription-tiers/{tierId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSubscriptionTier_WhenTierDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.DeleteAsync("/subscription-tiers/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchSubscriptionTiers_WhenQueryMatches_ShouldReturnFilteredResults()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SubscriptionTiers.RemoveRange(db.SubscriptionTiers);
            await db.SaveChangesAsync();

            db.SubscriptionTiers.AddRange(
                new SubscriptionTier { Name = "Premium Plus", Description = "Top", Price = 120m },
                new SubscriptionTier { Name = "Lite", Description = "Leve", Price = 15m }
            );

            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync("/subscription-tiers/search?q=Premium");
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("Premium Plus");
        content.Should().NotContain("Lite");
    }

    [Fact]
    public async Task SearchSubscriptionTiers_WhenQueryIsEmpty_ShouldReturnBadRequest()
    {
        var response = await _client.GetAsync("/subscription-tiers/search?q=");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}