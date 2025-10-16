using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.SubscriptionTiers;

public class SubscriptionTierDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "O preço é obrigatório.")]
    [Range(0.01, 999999999999.99, ErrorMessage = "Preço inválido.")]
    public decimal Price { get; set; }

    public SubscriptionTier ToEntity()
    {
        return new SubscriptionTier
        {
            Name = Name,
            Description = Description,
            Price = Price
        };
    }

    public static SubscriptionTierResponseDto FromEntity(SubscriptionTier tier)
    {
        return new SubscriptionTierResponseDto
        {
            Id = tier.Id,
            Name = tier.Name,
            Description = tier.Description,
            Price = tier.Price
        };
    }

    public void ApplyToEntity(SubscriptionTier tier)
    {
        tier.Name = Name;
        tier.Description = Description;
        tier.Price = Price;
    }
}
