using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PorquinhoApi.Models.Hateoas;
using PorquinhoApi.Services;
using Xunit;

namespace PorquinhoApi.UnitTests.Services;

public class HateoasLinkServiceTests
{
    [Fact]
    public void AddItemLinks_WhenResourceHasLinksProperty_ShouldAddSelfLink()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        var linkGenerator = new FakeLinkGenerator();
        linkGenerator.SetUri("GetById", "https://localhost:5001/subscriptions/1");

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        var service = new HateoasLinkService(linkGenerator, httpContextAccessor);
        var resource = new TestResource();

        // Act
        var result = service.AddItemLinks(
            resource,
            "GetById",
            new { id = 1 });

        // Assert
        Assert.Same(resource, result);
        result.Links.Should().HaveCount(1);
        result.Links[0].Href.Should().Be("https://localhost:5001/subscriptions/1");
        result.Links[0].Rel.Should().Be("self");
        result.Links[0].Method.Should().Be("GET");
    }

    [Fact]
    public void AddItemLinks_WhenExtrasAreProvided_ShouldAddSelfAndExtraLinks()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        var linkGenerator = new FakeLinkGenerator();
        linkGenerator.SetUri("GetById", "https://localhost:5001/subscriptions/1");
        linkGenerator.SetUri("UpdateSubscription", "https://localhost:5001/subscriptions/1/update");
        linkGenerator.SetUri("DeleteSubscription", "https://localhost:5001/subscriptions/1/delete");

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        var service = new HateoasLinkService(linkGenerator, httpContextAccessor);
        var resource = new TestResource();

        var extras = new (string rel, string method, string routeName, object? routeValues)[]
        {
            ("update", "PUT", "UpdateSubscription", null),
            ("delete", "DELETE", "DeleteSubscription", null)
        };

        // Act
        var result = service.AddItemLinks(
            resource,
            "GetById",
            new { id = 1 },
            extras);

        // Assert
        Assert.Same(resource, result);
        result.Links.Should().HaveCount(3);

        result.Links[0].Rel.Should().Be("self");
        result.Links[0].Method.Should().Be("GET");
        result.Links[0].Href.Should().Be("https://localhost:5001/subscriptions/1");

        result.Links[1].Rel.Should().Be("update");
        result.Links[1].Method.Should().Be("PUT");
        result.Links[1].Href.Should().Be("https://localhost:5001/subscriptions/1/update");

        result.Links[2].Rel.Should().Be("delete");
        result.Links[2].Method.Should().Be("DELETE");
        result.Links[2].Href.Should().Be("https://localhost:5001/subscriptions/1/delete");
    }

    [Fact]
    public void AddItemLinks_WhenResourceDoesNotHaveLinksProperty_ShouldReturnResourceUnchanged()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        var linkGenerator = new FakeLinkGenerator();

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        var service = new HateoasLinkService(linkGenerator, httpContextAccessor);
        var resource = new ResourceWithoutLinks();

        // Act
        var result = service.AddItemLinks(
            resource,
            "GetById",
            new { id = 1 });

        // Assert
        Assert.Same(resource, result);
    }

    [Fact]
    public void AddItemLinks_WhenLinksPropertyIsNotAListOfLink_ShouldReturnResourceUnchanged()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        var linkGenerator = new FakeLinkGenerator();

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        var service = new HateoasLinkService(linkGenerator, httpContextAccessor);
        var resource = new ResourceWithInvalidLinksProperty();

        // Act
        var result = service.AddItemLinks(
            resource,
            "GetById",
            new { id = 1 });

        // Assert
        Assert.Same(resource, result);
        resource.Links.Should().Be("invalid");
    }

    private class TestResource
    {
        public List<Link> Links { get; set; } = [];
    }

    private class ResourceWithoutLinks
    {
        public int Id { get; set; }
    }

    private class ResourceWithInvalidLinksProperty
    {
        public string Links { get; set; } = "invalid";
    }

    private sealed class FakeLinkGenerator : LinkGenerator
    {
        private readonly Dictionary<string, string> _uris = new();

        public void SetUri(string routeName, string uri)
        {
            _uris[routeName] = uri;
        }

        public override string? GetPathByAddress<TAddress>(
            HttpContext httpContext,
            TAddress address,
            RouteValueDictionary values,
            RouteValueDictionary? ambientValues = null,
            PathString? pathBase = null,
            FragmentString fragment = default,
            LinkOptions? options = null)
        {
            return null;
        }

        public override string? GetPathByAddress<TAddress>(
            TAddress address,
            RouteValueDictionary values,
            PathString pathBase = default,
            FragmentString fragment = default,
            LinkOptions? options = null)
        {
            return null;
        }

        public override string? GetUriByAddress<TAddress>(
            HttpContext httpContext,
            TAddress address,
            RouteValueDictionary values,
            RouteValueDictionary? ambientValues = null,
            string? scheme = null,
            HostString? host = null,
            PathString? pathBase = null,
            FragmentString fragment = default,
            LinkOptions? options = null)
        {
            if (address is string routeName && _uris.TryGetValue(routeName, out var uri))
                return uri;

            return string.Empty;
        }

        public override string? GetUriByAddress<TAddress>(
            TAddress address,
            RouteValueDictionary values,
            string scheme,
            HostString host,
            PathString pathBase = default,
            FragmentString fragment = default,
            LinkOptions? options = null)
        {
            if (address is string routeName && _uris.TryGetValue(routeName, out var uri))
                return uri;

            return string.Empty;
        }
    }
}