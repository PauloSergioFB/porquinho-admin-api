using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.SubscriptionTiers;

public record SubscriptionTierDto(
    [property: Required(ErrorMessage = "O nome é obrigatório.")]
    [property: MaxLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
    string Name,

    [property: MaxLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres.")]
    string? Description,

    [property: Required(ErrorMessage = "O preço é obrigatório.")]
    [property: Range(0.01, 999999999999.99, ErrorMessage = "Preço inválido.")]
    decimal Price,

    List<int>? FunctionalityIds
)
{
    public SubscriptionTier ToEntity() => new()
    {
        Name = Name,
        Description = Description,
        Price = Price
    };

    public void ApplyToEntity(SubscriptionTier tier)
    {
        tier.Name = Name;
        tier.Description = Description;
        tier.Price = Price;
    }
}
