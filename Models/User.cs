using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PorquinhoApi.Models;

[Table("P_USER")]
public class User
{
    [Key]
    [Column("USER_ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("FULL_NAME")]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Column("EMAIL")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("HASHED_PASSWORD")]
    [MaxLength(255)]
    public string HashedPassword { get; set; } = string.Empty;

    [Column("INCOME", TypeName = "NUMBER(14,2)")]
    public decimal? Income { get; set; }

    [Column("GENDER")]
    [MaxLength(10)]
    [RegularExpression("masculine|feminine|other", ErrorMessage = "Gender deve ser masculine, feminine ou other.")]
    public string? Gender { get; set; }

    [Column("PHONE_NUMBER")]
    public long? PhoneNumber { get; set; }

    [Column("BIRTHDAY", TypeName = "DATE")]
    public DateTime? Birthday { get; set; }

    [Required]
    [Column("PROFILE_PICTURE_URL")]
    [MaxLength(255)]
    public string ProfilePictureUrl { get; set; } = string.Empty;

    [Required]
    [Column("CREATED_AT", TypeName = "TIMESTAMP")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("UPDATED_AT", TypeName = "TIMESTAMP")]
    public DateTime? UpdatedAt { get; set; }

    // public int TimeZoneId { get; set; }

    // public int CountryId { get; set; }

    // public int FinanceObjectiveId { get; set; }

    // public TimeZone? TimeZone { get; set; }

    // public Country? Country { get; set; }

    // public FinanceObjective? FinanceObjective { get; set; }

}
