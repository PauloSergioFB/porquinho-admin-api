using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.SubscriptionTiers;

public class SubscriptionTierResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }

    public static SubscriptionTierResponseDto FromEntity(SubscriptionTier subscriptionTier)
    {
        return new SubscriptionTierResponseDto
        {
            Id = subscriptionTier.Id,
            Name = subscriptionTier.Name,
            Description = subscriptionTier.Description,
            Price = subscriptionTier.Price
        };
    }
}
