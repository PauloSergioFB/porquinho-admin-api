using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PorquinhoApi.Models;

[Table("P_FUNCTIONALITY")]
public class Functionality
{
    [Key]
    [Column("FUNCTIONALITY_ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("NAME")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("CODE")]
    public string Code { get; set; } = string.Empty;
}
