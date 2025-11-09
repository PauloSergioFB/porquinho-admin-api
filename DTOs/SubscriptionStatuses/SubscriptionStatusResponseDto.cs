using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.SubscriptionStatuses;

public record SubscriptionStatusResponseDto(
    int Id,
    string Description,
    string Code
)
{
    public static SubscriptionStatusResponseDto FromEntity(SubscriptionStatus subscriptionStatus) =>
        new(
            subscriptionStatus.Id,
            subscriptionStatus.Description,
            subscriptionStatus.Code
        );
}
