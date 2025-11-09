using PorquinhoApi.DTOs.Functionalities;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.SubscriptionTiers;

public record SubscriptionTierResponseDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    List<FunctionalityResponseDto> Functionalities
)
{
    public static SubscriptionTierResponseDto FromEntity(SubscriptionTier subscriptionTier) =>
        new(
            subscriptionTier.Id,
            subscriptionTier.Name,
            subscriptionTier.Description,
            subscriptionTier.Price,
            subscriptionTier.Functionalities?
                .Select(FunctionalityResponseDto.FromEntity)
                .ToList() ?? []
        );
}
