using PorquinhoApi.Utils;

namespace PorquinhoApi.Services;

public static class PaginationResponseBuilder
{
    public static PagedResponse<T> Build<T>(
        IEnumerable<T> items,
        int page,
        int pageSize,
        int totalItems,
        string routeBase,
        HttpContext http)
    {
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var baseUrl = $"{http.Request.Scheme}://{http.Request.Host}/{routeBase.TrimStart('/')}";

        string Link(int targetPage) => $"{baseUrl}?page={targetPage}&pageSize={pageSize}";

        var links = new List<object>
        {
            new { rel = "self", method = "GET", href = Link(page) }
        };

        if (page < totalPages)
            links.Add(new { rel = "next", method = "GET", href = Link(page + 1) });

        if (page > 1)
            links.Add(new { rel = "prev", method = "GET", href = Link(page - 1) });

        return new PagedResponse<T>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            Items = items,
            Links = links
        };
    }
}
