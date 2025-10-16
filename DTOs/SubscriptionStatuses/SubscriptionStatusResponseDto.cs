using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.SubscriptionStatuses;

public class SubscriptionStatusResponseDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public static SubscriptionStatusResponseDto FromEntity(SubscriptionStatus subscriptionStatus)
    {
        return new SubscriptionStatusResponseDto
        {
            Id = subscriptionStatus.Id,
            Description = subscriptionStatus.Description,
            Code = subscriptionStatus.Code
        };
    }
}