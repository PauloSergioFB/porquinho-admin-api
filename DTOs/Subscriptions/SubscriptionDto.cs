using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Subscriptions;

public record SubscriptionDto(
    [property: Required(ErrorMessage = "O ID do usuário é obrigatório.")]
    int UserId,

    [property: Required(ErrorMessage = "O ID do plano de assinatura é obrigatório.")]
    int SubscriptionTierId,

    [property: Required(ErrorMessage = "O ID do status da assinatura é obrigatório.")]
    int SubscriptionStatusId
)
{
    public Subscription ToEntity() => new()
    {
        UserId = UserId,
        SubscriptionTierId = SubscriptionTierId,
        SubscriptionStatusId = SubscriptionStatusId
    };

    public void ApplyToEntity(Subscription subscription)
    {
        subscription.UserId = UserId;
        subscription.SubscriptionTierId = SubscriptionTierId;
        subscription.SubscriptionStatusId = SubscriptionStatusId;
    }
}
