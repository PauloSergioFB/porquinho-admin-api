using PorquinhoApi.Models;
using PorquinhoApi.Models.Hateoas;

namespace PorquinhoApi.DTOs.Users;

public record UserResponseDto(
    int Id,
    string FullName,
    string Email,
    decimal? Income,
    string? Gender,
    long? PhoneNumber,
    DateTime? Birthday,
    string? ProfilePictureUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt
)
{
    public static UserResponseDto FromEntity(User user) =>
        new(
            user.Id,
            user.FullName,
            user.Email,
            user.Income,
            user.Gender,
            user.PhoneNumber,
            user.Birthday,
            user.ProfilePictureUrl,
            user.CreatedAt,
            user.UpdatedAt
        );

    public List<Link> Links { get; init; } = [];
}
