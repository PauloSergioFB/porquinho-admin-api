using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PorquinhoApi.Filters;

public partial class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var dto = context.Arguments.OfType<T>().FirstOrDefault();
        if (dto is not null)
        {
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), results, true);

            if (!isValid)
            {
                var errors = results.ToDictionary(
                    r => ToSnakeCase(r.MemberNames.FirstOrDefault() ?? "error"),
                    r => r.ErrorMessage ?? "Valor inválido."
                );

                return Results.BadRequest(errors);
            }
        }

        return await next(context);
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return MyRegex().Replace(input, "_$1").ToLower();
    }

    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex MyRegex();
}
