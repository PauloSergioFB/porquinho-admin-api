using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Functionalities;

public class FunctionalityDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O código é obrigatório.")]
    [MaxLength(20, ErrorMessage = "O código deve ter no máximo 20 caracteres.")]
    public string Code { get; set; } = string.Empty;

    public Functionality ToEntity()
    {
        return new Functionality
        {
            Name = Name,
            Code = Code
        };
    }

    public static FunctionalityResponseDto FromEntity(Functionality functionality)
    {
        return new FunctionalityResponseDto
        {
            Id = functionality.Id,
            Name = functionality.Name,
            Code = functionality.Code
        };
    }

    public void ApplyToEntity(Functionality functionality)
    {
        functionality.Name = Name;
        functionality.Code = Code;
    }
}