using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PorquinhoApi.Models;

[Table("P_SUBSCRIPTION")]
public class Subscription
{
    [Key]
    [Column("SUBSCRIPTION_ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("USER_ID")]
    public int UserId { get; set; }

    [Required]
    [Column("SUBSCRIPTION_TIER_ID")]
    public int SubscriptionTierId { get; set; }

    [Required]
    [Column("START_DATE")]
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    [Column("END_DATE")]
    public DateTime? EndDate { get; set; }

    [Required]
    [Column("SUBSCRIPTION_STATUS_ID")]
    public int SubscriptionStatusId { get; set; }

    [Required]
    [Column("CREATED_AT")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("UPDATED_AT")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(SubscriptionTierId))]
    public SubscriptionTier SubscriptionTier { get; set; } = null!;

    [ForeignKey(nameof(SubscriptionStatusId))]
    public SubscriptionStatus SubscriptionStatus { get; set; } = null!;
}