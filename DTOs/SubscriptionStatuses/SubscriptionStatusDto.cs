using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.SubscriptionStatuses;

public record SubscriptionStatusDto(
    [property: Required(ErrorMessage = "A descrição é obrigatória.")]
    [property: MaxLength(50, ErrorMessage = "A descrição deve ter no máximo 50 caracteres.")]
    string Description,

    [property: Required(ErrorMessage = "O código é obrigatório.")]
    [property: MaxLength(20, ErrorMessage = "O código deve ter no máximo 20 caracteres.")]
    string Code
)
{
    public SubscriptionStatus ToEntity() => new()
    {
        Description = Description,
        Code = Code
    };

    public void ApplyToEntity(SubscriptionStatus status)
    {
        status.Description = Description;
        status.Code = Code;
    }
}
