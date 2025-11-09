using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Functionalities;

public record FunctionalityResponseDto(
    int Id,
    string Name,
    string Code
)
{
    public static FunctionalityResponseDto FromEntity(Functionality functionality) =>
        new(
            functionality.Id,
            functionality.Name,
            functionality.Code
        );
}
