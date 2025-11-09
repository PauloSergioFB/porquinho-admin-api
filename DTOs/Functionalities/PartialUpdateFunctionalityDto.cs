using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Functionalities;

public record PartialUpdateFunctionalityDto(
    [property: MaxLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
    string? Name,

    [property: MaxLength(20, ErrorMessage = "O código deve ter no máximo 20 caracteres.")]
    string? Code
)
{
    public void ApplyToEntity(Functionality functionality)
    {
        if (Name is not null)
            functionality.Name = Name;

        if (Code is not null)
            functionality.Code = Code;
    }
}
