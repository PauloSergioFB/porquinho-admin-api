using FluentAssertions;
using Microsoft.AspNetCore.Http;
using PorquinhoApi.Services;
using Xunit;

namespace PorquinhoApi.UnitTests.Services;

public class PaginationResponseBuilderTests
{
    [Fact]
    public void Build_WhenCalled_ShouldReturnPagedResponseWithCorrectMetadata()
    {
        // Arrange
        var items = new List<string> { "item1", "item2" };
        var httpContext = CreateHttpContext();
        const int page = 2;
        const int pageSize = 2;
        const int totalItems = 5;
        const string routeBase = "subscriptions";

        // Act
        var result = PaginationResponseBuilder.Build(
            items,
            page,
            pageSize,
            totalItems,
            routeBase,
            httpContext
        );

        // Assert
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(2);
        result.TotalItems.Should().Be(5);
        result.TotalPages.Should().Be(3);
        result.Items.Should().BeEquivalentTo(items);
        result.Links.Should().NotBeNull();
        result.Links.Should().HaveCount(3);
    }

    [Fact]
    public void Build_WhenPageIsFirst_ShouldIncludeSelfAndNextOnly()
    {
        // Arrange
        var items = new List<string> { "item1", "item2" };
        var httpContext = CreateHttpContext();
        const int page = 1;
        const int pageSize = 2;
        const int totalItems = 5;
        const string routeBase = "subscriptions";

        // Act
        var result = PaginationResponseBuilder.Build(
            items,
            page,
            pageSize,
            totalItems,
            routeBase,
            httpContext
        );

        // Assert
        result.TotalPages.Should().Be(3);
        result.Links.Should().HaveCount(2);

        var linksAsText = result.Links.Select(l => l!.ToString()).ToList();

        linksAsText.Should().Contain(x => x!.Contains("rel = self"));
        linksAsText.Should().Contain(x => x.Contains("rel = next"));
        linksAsText.Should().NotContain(x => x.Contains("rel = prev"));
    }

    [Fact]
    public void Build_WhenPageIsMiddle_ShouldIncludeSelfNextAndPrev()
    {
        // Arrange
        var items = new List<string> { "item3", "item4" };
        var httpContext = CreateHttpContext();
        const int page = 2;
        const int pageSize = 2;
        const int totalItems = 6;
        const string routeBase = "subscriptions";

        // Act
        var result = PaginationResponseBuilder.Build(
            items,
            page,
            pageSize,
            totalItems,
            routeBase,
            httpContext
        );

        // Assert
        result.TotalPages.Should().Be(3);
        result.Links.Should().HaveCount(3);

        var linksAsText = result.Links.Select(l => l!.ToString()).ToList();

        linksAsText.Should().Contain(x => x!.Contains("rel = self"));
        linksAsText.Should().Contain(x => x.Contains("rel = next"));
        linksAsText.Should().Contain(x => x.Contains("rel = prev"));
    }

    [Fact]
    public void Build_WhenPageIsLast_ShouldIncludeSelfAndPrevOnly()
    {
        // Arrange
        var items = new List<string> { "item5", "item6" };
        var httpContext = CreateHttpContext();
        const int page = 3;
        const int pageSize = 2;
        const int totalItems = 6;
        const string routeBase = "subscriptions";

        // Act
        var result = PaginationResponseBuilder.Build(
            items,
            page,
            pageSize,
            totalItems,
            routeBase,
            httpContext
        );

        // Assert
        result.TotalPages.Should().Be(3);
        result.Links.Should().HaveCount(2);

        var linksAsText = result.Links.Select(l => l!.ToString()).ToList();

        linksAsText.Should().Contain(x => x!.Contains("rel = self"));
        linksAsText.Should().Contain(x => x.Contains("rel = prev"));
        linksAsText.Should().NotContain(x => x.Contains("rel = next"));
    }

    private static HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("localhost:5001");

        return context;
    }
}