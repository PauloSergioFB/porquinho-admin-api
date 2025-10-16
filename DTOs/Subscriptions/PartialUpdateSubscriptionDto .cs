using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Subscriptions;

public class PartialUpdateSubscriptionDto
{
    public int? UserId { get; set; }
    public int? SubscriptionTierId { get; set; }
    public int? SubscriptionStatusId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public void ApplyToEntity(Subscription subscription)
    {
        if (UserId is not null)
            subscription.UserId = UserId.Value;

        if (SubscriptionTierId is not null)
            subscription.SubscriptionTierId = SubscriptionTierId.Value;

        if (SubscriptionStatusId is not null)
            subscription.SubscriptionStatusId = SubscriptionStatusId.Value;

        if (StartDate is not null)
            subscription.StartDate = StartDate.Value;

        if (EndDate is not null)
            subscription.EndDate = EndDate;
    }
}