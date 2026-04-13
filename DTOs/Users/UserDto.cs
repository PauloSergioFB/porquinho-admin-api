using System.ComponentModel.DataAnnotations;
using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Users;

public record UserDto(
    [property: Required(ErrorMessage = "O nome completo é obrigatório.")]
    [property: MaxLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres.")]
    string FullName,

    [property: Required(ErrorMessage = "O e-mail é obrigatório.")]
    [property: EmailAddress(ErrorMessage = "E-mail inválido.")]
    [property: MaxLength(255)]
    string Email,

    [property: Required(ErrorMessage = "A senha é obrigatória.")]
    [property: MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    [property: MaxLength(255)]
    string Password,

    [property: Range(0, 999999999999.99, ErrorMessage = "Renda inválida.")]
    decimal? Income,

    [property: RegularExpression("masculine|feminine|other", ErrorMessage = "Gênero deve ser masculine, feminine ou other.")]
    string? Gender,

    [property: Range(10000000000, 999999999999, ErrorMessage = "Número de telefone inválido.")]
    long? PhoneNumber,

    DateTime? Birthday,

    [property: Url(ErrorMessage = "A URL da foto de perfil é inválida.")]
    [property: MaxLength(255)]
    string ProfilePictureUrl
)
{
    public User ToEntity() => new()
    {
        FullName = FullName,
        Email = Email,
        HashedPassword = Password,
        Income = Income,
        Gender = Gender,
        PhoneNumber = PhoneNumber,
        Birthday = Birthday,
        ProfilePictureUrl = ProfilePictureUrl
    };

    public void ApplyToEntity(User user)
    {
        user.FullName = FullName;
        user.Email = Email;
        user.Income = Income;
        user.Gender = Gender;
        user.PhoneNumber = PhoneNumber;
        user.Birthday = Birthday;
        user.ProfilePictureUrl = ProfilePictureUrl;
    }
}
