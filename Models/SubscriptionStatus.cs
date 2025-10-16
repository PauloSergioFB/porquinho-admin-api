using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PorquinhoApi.Models;

[Table("P_SUBSCRIPTION_STATUS")]
public class SubscriptionStatus
{
    [Key]
    [Column("SUBSCRIPTION_STATUS_ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("DESCRIPTION")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("CODE")]
    public string Code { get; set; } = string.Empty;
}
