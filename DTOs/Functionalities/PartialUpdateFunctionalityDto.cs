using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Functionalities;

public class PartialUpdateFunctionalityDto
{
    [MaxLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
    public string? Name { get; set; }

    [MaxLength(20, ErrorMessage = "O código deve ter no máximo 20 caracteres.")]
    public string? Code { get; set; }

    public void ApplyToEntity(Functionality functionality)
    {
        if (Name is not null)
            functionality.Name = Name;

        if (Code is not null)
            functionality.Code = Code;
    }
}