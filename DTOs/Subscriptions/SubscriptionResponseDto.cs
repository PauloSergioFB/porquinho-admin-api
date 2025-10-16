using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Subscriptions;

public class SubscriptionResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SubscriptionTierId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int SubscriptionStatusId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static SubscriptionResponseDto FromEntity(Subscription subscription)
    {
        return new SubscriptionResponseDto
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            SubscriptionTierId = subscription.SubscriptionTierId,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            SubscriptionStatusId = subscription.SubscriptionStatusId,
            CreatedAt = subscription.CreatedAt,
            UpdatedAt = subscription.UpdatedAt
        };
    }
}