using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PorquinhoApi.Data;
using PorquinhoApi.IntegrationTests.Infrastructure;
using PorquinhoApi.Models;
using Xunit;

namespace PorquinhoApi.IntegrationTests.Endpoints;

public class SubscriptionsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SubscriptionsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<(int userId, int tierId, int statusId)> SeedDependencies()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Subscriptions.RemoveRange(db.Subscriptions);
        db.SubscriptionTiers.RemoveRange(db.SubscriptionTiers);
        db.SubscriptionStatuses.RemoveRange(db.SubscriptionStatuses);
        db.Users.RemoveRange(db.Users);

        await db.SaveChangesAsync();

        var user = new User
        {
            FullName = "User Test",
            Email = $"user_{Guid.NewGuid():N}@email.com",
            HashedPassword = "123456"
        };

        var tier = new SubscriptionTier
        {
            Name = $"Plano {Guid.NewGuid():N}",
            Price = 10
        };

        var status = new SubscriptionStatus
        {
            Description = $"Ativo {Guid.NewGuid():N}",
            Code = $"ACTIVE_{Guid.NewGuid():N}".Substring(0, 20)
        };

        db.Users.Add(user);
        db.SubscriptionTiers.Add(tier);
        db.SubscriptionStatuses.Add(status);

        await db.SaveChangesAsync();

        return (user.Id, tier.Id, status.Id);
    }

    [Fact]
    public async Task GetAllSubscriptions_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/subscriptions?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateSubscription_WhenValid_ShouldReturnCreated()
    {
        var (userId, tierId, statusId) = await SeedDependencies();

        var request = new
        {
            user_id = userId,
            subscription_tier_id = tierId,
            subscription_status_id = statusId
        };

        var response = await _client.PostAsJsonAsync("/subscriptions", request);
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Created, content);
    }

    [Fact]
    public async Task GetSubscriptionById_WhenExists_ShouldReturnOk()
    {
        var (userId, tierId, statusId) = await SeedDependencies();

        int subscriptionId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var subscription = new Subscription
            {
                UserId = userId,
                SubscriptionTierId = tierId,
                SubscriptionStatusId = statusId,
                StartDate = DateTime.UtcNow
            };

            db.Subscriptions.Add(subscription);
            await db.SaveChangesAsync();

            subscriptionId = subscription.Id;
        }

        var response = await _client.GetAsync($"/subscriptions/{subscriptionId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSubscriptionById_WhenNotExists_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync("/subscriptions/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateSubscription_WhenExists_ShouldReturnOk()
    {
        var (userId, tierId, statusId) = await SeedDependencies();

        int subscriptionId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var subscription = new Subscription
            {
                UserId = userId,
                SubscriptionTierId = tierId,
                SubscriptionStatusId = statusId,
                StartDate = DateTime.UtcNow
            };

            db.Subscriptions.Add(subscription);
            await db.SaveChangesAsync();

            subscriptionId = subscription.Id;
        }

        var request = new
        {
            user_id = userId,
            subscription_tier_id = tierId,
            subscription_status_id = statusId
        };

        var response = await _client.PutAsJsonAsync($"/subscriptions/{subscriptionId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateSubscription_WhenNotExists_ShouldReturnNotFound()
    {
        var (userId, tierId, statusId) = await SeedDependencies();

        var request = new
        {
            user_id = userId,
            subscription_tier_id = tierId,
            subscription_status_id = statusId
        };

        var response = await _client.PutAsJsonAsync("/subscriptions/999999", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PartialUpdateSubscription_WhenExists_ShouldReturnOk()
    {
        var (userId, tierId, statusId) = await SeedDependencies();

        int subscriptionId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var subscription = new Subscription
            {
                UserId = userId,
                SubscriptionTierId = tierId,
                SubscriptionStatusId = statusId,
                StartDate = DateTime.UtcNow
            };

            db.Subscriptions.Add(subscription);
            await db.SaveChangesAsync();

            subscriptionId = subscription.Id;
        }

        var request = new
        {
            subscription_status_id = statusId
        };

        var response = await _client.PatchAsJsonAsync($"/subscriptions/{subscriptionId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteSubscription_WhenExists_ShouldReturnNoContent()
    {
        var (userId, tierId, statusId) = await SeedDependencies();

        int subscriptionId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var subscription = new Subscription
            {
                UserId = userId,
                SubscriptionTierId = tierId,
                SubscriptionStatusId = statusId,
                StartDate = DateTime.UtcNow
            };

            db.Subscriptions.Add(subscription);
            await db.SaveChangesAsync();

            subscriptionId = subscription.Id;
        }

        var deleteResponse = await _client.DeleteAsync($"/subscriptions/{subscriptionId}");
        var getResponse = await _client.GetAsync($"/subscriptions/{subscriptionId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}