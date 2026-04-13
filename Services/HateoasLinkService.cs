using PorquinhoApi.Models.Hateoas;

namespace PorquinhoApi.Services;

public interface IHateoasLinkService
{
    T AddItemLinks<T>(
        T resource,
        string routeNameGetById,
        object routeValuesForItem,
        (string rel, string method, string routeName, object? routeValues)[]? extras = null
    ) where T : class;
}

public class HateoasLinkService(LinkGenerator linkGen, IHttpContextAccessor http) : IHateoasLinkService
{
    private HttpContext Context => http.HttpContext!;

    private string BuildUrl(string routeName, object? values = null)
        => linkGen.GetUriByName(Context, routeName, values) ?? string.Empty;

    public T AddItemLinks<T>(
        T resource,
        string routeNameGetById,
        object routeValuesForItem,
        (string rel, string method, string routeName, object? routeValues)[]? extras = null
    ) where T : class
    {
        if (resource is not { } obj) return resource;

        var prop = obj.GetType().GetProperty("Links");
        if (prop?.GetValue(obj) is not List<Link> links)
            return resource;

        links.Add(new Link(BuildUrl(routeNameGetById, routeValuesForItem), "self", "GET"));

        if (extras is not null)
        {
            foreach (var (rel, method, routeName, routeValues) in extras)
            {
                links.Add(new Link(BuildUrl(routeName, routeValues ?? routeValuesForItem), rel, method));
            }
        }

        return resource;
    }
}
