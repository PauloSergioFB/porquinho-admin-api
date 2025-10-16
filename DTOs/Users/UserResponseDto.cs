using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Users;

public class UserResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal? Income { get; set; }
    public string? Gender { get; set; }
    public long? PhoneNumber { get; set; }
    public DateTime? Birthday { get; set; }
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static UserResponseDto FromEntity(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Income = user.Income,
            Gender = user.Gender,
            PhoneNumber = user.PhoneNumber,
            Birthday = user.Birthday,
            ProfilePictureUrl = user.ProfilePictureUrl,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
