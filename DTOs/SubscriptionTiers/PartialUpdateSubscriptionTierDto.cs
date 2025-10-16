using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.SubscriptionTiers;

public class PartialUpdateSubscriptionTierDto
{
    [MaxLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
    public string? Name { get; set; }

    [MaxLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres.")]
    public string? Description { get; set; }

    [Range(0.01, 999999999999.99, ErrorMessage = "Preço inválido.")]
    public decimal? Price { get; set; }

    public void ApplyToEntity(SubscriptionTier tier)
    {
        if (Name is not null)
            tier.Name = Name;

        if (Description is not null)
            tier.Description = Description;

        if (Price is not null)
            tier.Price = Price.Value;
    }
}
