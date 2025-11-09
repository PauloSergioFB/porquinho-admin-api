using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Users;

public record PartialUpdateUserDto(
    [property: MaxLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres.")]
    string? FullName,

    [property: EmailAddress(ErrorMessage = "E-mail inválido.")]
    [property: MaxLength(255)]
    string? Email,

    [property: MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    [property: MaxLength(255)]
    string? Password,

    [property: Range(0, 999999999999.99, ErrorMessage = "Renda inválida.")]
    decimal? Income,

    [property: RegularExpression("masculine|feminine|other", ErrorMessage = "Gênero deve ser masculine, feminine ou other.")]
    string? Gender,

    [property: Range(10000000000, 999999999999, ErrorMessage = "Número de telefone inválido.")]
    long? PhoneNumber,

    DateTime? Birthday,

    [property: Url(ErrorMessage = "A URL da foto de perfil é inválida.")]
    [property: MaxLength(255)]
    string? ProfilePictureUrl
)
{
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
