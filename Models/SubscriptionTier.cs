using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PorquinhoApi.Models;

[Table("P_SUBSCRIPTION_TIER")]
public class SubscriptionTier
{
    [Key]
    [Column("SUBSCRIPTION_TIER_ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("NAME")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    [Column("DESCRIPTION")]
    public string? Description { get; set; }

    [Required]
    [Column("PRICE", TypeName = "NUMBER(14,2)")]
    public decimal Price { get; set; }
}