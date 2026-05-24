namespace PorquinhoApi.DTOs.Transactions;

public class UpdateImportedTransactionDto
{
    public decimal TransactionValue { get; set; }

    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; }

    public bool HasOccurred { get; set; }

    public bool IsAutoConfirmed { get; set; }

    public string? Observation { get; set; }
}