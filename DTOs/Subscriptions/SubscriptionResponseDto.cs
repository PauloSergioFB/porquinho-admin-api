using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Subscriptions;

public record SubscriptionResponseDto(
    int Id,
    int UserId,
    int SubscriptionTierId,
    DateTime StartDate,
    DateTime? EndDate,
    int SubscriptionStatusId,
    DateTime CreatedAt,
    DateTime? UpdatedAt
)
{
    public static SubscriptionResponseDto FromEntity(Subscription subscription) =>
        new(
            subscription.Id,
            subscription.UserId,
            subscription.SubscriptionTierId,
            subscription.StartDate,
            subscription.EndDate,
            subscription.SubscriptionStatusId,
            subscription.CreatedAt,
            subscription.UpdatedAt
        );
}
