using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.SubscriptionStatuses;

public class SubscriptionStatusDto
{
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [MaxLength(50, ErrorMessage = "A descrição deve ter no máximo 50 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "O código é obrigatório.")]
    [MaxLength(20, ErrorMessage = "O código deve ter no máximo 20 caracteres.")]
    public string Code { get; set; } = string.Empty;

    public SubscriptionStatus ToEntity()
    {
        return new SubscriptionStatus
        {
            Description = Description,
            Code = Code
        };
    }

    public void ApplyToEntity(SubscriptionStatus status)
    {
        status.Description = Description;
        status.Code = Code;
    }
}