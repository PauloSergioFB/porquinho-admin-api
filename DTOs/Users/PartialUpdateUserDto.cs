using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Users;

public class PartialUpdateUserDto
{
    [MaxLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres.")]
    public string? FullName { get; set; }

    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [MaxLength(255)]
    public string? Email { get; set; }

    [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    [MaxLength(255)]
    public string? Password { get; set; }

    [Range(0, 999999999999.99, ErrorMessage = "Renda inválida.")]
    public decimal? Income { get; set; }

    [RegularExpression("masculine|feminine|other", ErrorMessage = "Gênero deve ser masculine, feminine ou other.")]
    public string? Gender { get; set; }

    [Range(10000000000, 999999999999, ErrorMessage = "Número de telefone inválido.")]
    public long? PhoneNumber { get; set; }

    public DateTime? Birthday { get; set; }

    [Url(ErrorMessage = "A URL da foto de perfil é inválida.")]
    [MaxLength(255)]
    public string? ProfilePictureUrl { get; set; }

    public void ApplyToEntity(User user)
    {
        if (FullName is not null)
            user.FullName = FullName;

        if (Email is not null)
            user.Email = Email;

        if (Password is not null)
            user.HashedPassword = Password;

        if (Income is not null)
            user.Income = Income;

        if (Gender is not null)
            user.Gender = Gender;

        if (PhoneNumber is not null)
            user.PhoneNumber = PhoneNumber;

        if (Birthday is not null)
            user.Birthday = Birthday;

        if (ProfilePictureUrl is not null)
            user.ProfilePictureUrl = ProfilePictureUrl;
    }
}
