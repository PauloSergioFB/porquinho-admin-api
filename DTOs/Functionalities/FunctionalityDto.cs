using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Functionalities;

public record FunctionalityDto(
    [property: Required(ErrorMessage = "O nome é obrigatório.")]
    [property: MaxLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
    string Name,

    [property: Required(ErrorMessage = "O código é obrigatório.")]
    [property: MaxLength(20, ErrorMessage = "O código deve ter no máximo 20 caracteres.")]
    string Code
)
{
    public Functionality ToEntity() => new()
    {
        Name = Name,
        Code = Code
    };

    public void ApplyToEntity(Functionality functionality)
    {
        functionality.Name = Name;
        functionality.Code = Code;
    }
}