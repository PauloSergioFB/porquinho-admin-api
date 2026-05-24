using PorquinhoApi.Models;

namespace PorquinhoApi.DTOs.Transactions;

public class ImportedTransactionResponseDto
{
    public string Id { get; set; } = string.Empty;

    public int OracleTransactionId { get; set; }

    public decimal TransactionValue { get; set; }

    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; }

    public bool HasOccurred { get; set; }

    public bool IsAutoConfirmed { get; set; }

    public string? Observation { get; set; }

    public DateTime OracleCreatedAt { get; set; }

    public DateTime? OracleUpdatedAt { get; set; }

    public DateTime ImportedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public static ImportedTransactionResponseDto FromEntity(
        ImportedTransaction transaction)
    {
        return new ImportedTransactionResponseDto
        {
            Id = transaction.Id,
            OracleTransactionId = transaction.OracleTransactionId,
            TransactionValue = transaction.TransactionValue,
            Description = transaction.Description,
            TransactionDate = transaction.TransactionDate,
            HasOccurred = transaction.HasOccurred,
            IsAutoConfirmed = transaction.IsAutoConfirmed,
            Observation = transaction.Observation,
            OracleCreatedAt = transaction.OracleCreatedAt,
            OracleUpdatedAt = transaction.OracleUpdatedAt,
            ImportedAt = transaction.ImportedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }
}