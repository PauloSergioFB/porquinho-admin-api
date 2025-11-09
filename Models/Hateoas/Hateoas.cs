namespace PorquinhoApi.Models.Hateoas;

public class Link
{
    public string Href { get; set; } = string.Empty;
    public string Rel { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;

    public Link() { }
    public Link(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
}

public class LinkResourceBase
{
    public List<Link> Links { get; set; } = new();
}
