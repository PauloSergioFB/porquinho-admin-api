using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.SubscriptionStatuses;

public class PartialUpdateSubscriptionStatusDto
{
    [MaxLength(50, ErrorMessage = "A descrição deve ter no máximo 50 caracteres.")]
    public string? Description { get; set; }

    [MaxLength(20, ErrorMessage = "O código deve ter no máximo 20 caracteres.")]
    public string? Code { get; set; }

    public void ApplyToEntity(SubscriptionStatus status)
    {
        if (Description is not null)
            status.Description = Description;

        if (Code is not null)
            status.Code = Code;
    }
}