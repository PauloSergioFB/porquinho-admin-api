using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Functionalities;

public class FunctionalityResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public static FunctionalityResponseDto FromEntity(Functionality functionality)
    {
        return new FunctionalityResponseDto
        {
            Id = functionality.Id,
            Name = functionality.Name,
            Code = functionality.Code
        };
    }
}