using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PorquinhoApi.Models;

[Table("P_TRANSACTION")]
public class Transaction
{
    [Key]
    [Column("TRANSACTION_ID")]
    public int TransactionId { get; set; }

    [Column("TRANSACTION_VALUE")]
    public decimal TransactionValue { get; set; }

    [Column("DESCRIPTION")]
    public string? Description { get; set; }

    [Column("TRANSACTION_DATE")]
    public DateTime TransactionDate { get; set; }

    [Column("HAS_OCCURRED")]
    public bool HasOccurred { get; set; }

    [Column("IS_AUTO_CONFIRMED")]
    public bool IsAutoConfirmed { get; set; }

    [Column("OBSERVATION")]
    public string? Observation { get; set; }

    [Column("CREATED_AT")]
    public DateTime CreatedAt { get; set; }

    [Column("UPDATED_AT")]
    public DateTime? UpdatedAt { get; set; }
}