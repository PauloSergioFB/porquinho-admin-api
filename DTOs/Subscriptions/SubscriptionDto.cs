using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Subscriptions;

public class SubscriptionDto
{
    [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "O ID do plano de assinatura é obrigatório.")]
    public int SubscriptionTierId { get; set; }

    [Required(ErrorMessage = "O ID do status da assinatura é obrigatório.")]
    public int SubscriptionStatusId { get; set; }

    public Subscription ToEntity()
    {
        return new Subscription
        {
            UserId = UserId,
            SubscriptionTierId = SubscriptionTierId,
            SubscriptionStatusId = SubscriptionStatusId
        };
    }

    public void ApplyToEntity(Subscription subscription)
    {
        subscription.UserId = UserId;
        subscription.SubscriptionTierId = SubscriptionTierId;
        subscription.SubscriptionStatusId = SubscriptionStatusId;
    }
}