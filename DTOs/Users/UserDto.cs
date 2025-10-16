using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Users;

public class UserDto
{
    [Required(ErrorMessage = "O nome completo é obrigatório.")]
    [MaxLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    [Range(0, 999999999999.99, ErrorMessage = "Renda inválida.")]
    public decimal? Income { get; set; }

    [RegularExpression("masculine|feminine|other", ErrorMessage = "Gênero deve ser masculine, feminine ou other.")]
    public string? Gender { get; set; }

    [Range(10000000000, 999999999999, ErrorMessage = "Número de telefone inválido.")]
    public long? PhoneNumber { get; set; }

    public DateTime? Birthday { get; set; }

    [Required(ErrorMessage = "A URL da foto de perfil é obrigatória.")]
    [Url(ErrorMessage = "A URL da foto de perfil é inválida.")]
    [MaxLength(255)]
    public string ProfilePictureUrl { get; set; } = string.Empty;

    public User ToEntity()
    {
        return new User
        {
            FullName = FullName,
            Email = Email,
            HashedPassword = Password,
            Income = Income,
            Gender = Gender,
            PhoneNumber = PhoneNumber,
            Birthday = Birthday,
            ProfilePictureUrl = ProfilePictureUrl ?? string.Empty,
            CreatedAt = DateTime.Now
        };
    }

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

    public void ApplyToEntity(User user)
    {
        user.FullName = FullName;
        user.Email = Email;
        user.Income = Income;
        user.Gender = Gender;
        user.PhoneNumber = PhoneNumber;
        user.Birthday = Birthday;
        user.ProfilePictureUrl = ProfilePictureUrl ?? user.ProfilePictureUrl;
        user.UpdatedAt = DateTime.Now;
    }
}