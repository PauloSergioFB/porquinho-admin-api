using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PorquinhoApi.Data;
using PorquinhoApi.DTOs.Transactions;
using PorquinhoApi.Models;
using PorquinhoApi.Services;

namespace PorquinhoApi.Endpoints;

public static class ImportedTransactionsEndpoints
{
    public static void MapImportedTransactionEndpoints(
        this WebApplication app)
    {
        var transactions = app.MapGroup("/transactions")
            .WithTags("Imported Transactions")
            .RequireAuthorization();

        transactions.MapGet("/", GetAllTransactions)
            .WithName("GetAllTransactions")
            .WithSummary("Retorna todas as transações importadas")
            .Produces<Ok<List<ImportedTransactionResponseDto>>>(
                StatusCodes.Status200OK
            );

        transactions.MapGet("/{id}", GetTransactionById)
            .WithName("GetTransactionById")
            .WithSummary("Retorna uma transação importada")
            .Produces<Ok<ImportedTransactionResponseDto>>(
                StatusCodes.Status200OK
            )
            .Produces<NotFound>(
                StatusCodes.Status404NotFound
            );

        transactions.MapPost("/", CreateTransaction)
            .WithName("CreateTransaction")
            .WithSummary("Cria uma transação importada")
            .Produces<Created<ImportedTransactionResponseDto>>(
                StatusCodes.Status201Created
            );

        transactions.MapPut("/{id}", UpdateTransaction)
            .WithName("UpdateTransaction")
            .WithSummary("Atualiza uma transação importada")
            .Produces<Ok<ImportedTransactionResponseDto>>(
                StatusCodes.Status200OK
            )
            .Produces<NotFound>(
                StatusCodes.Status404NotFound
            );

        transactions.MapDelete("/{id}", DeleteTransaction)
            .WithName("DeleteTransaction")
            .WithSummary("Remove uma transação importada")
            .Produces<NoContent>(
                StatusCodes.Status204NoContent
            )
            .Produces<NotFound>(
                StatusCodes.Status404NotFound
            );

        transactions.MapDelete("/", DeleteAllTransactions)
            .WithName("DeleteAllTransactions")
            .WithSummary("Remove todas as transações importadas")
            .Produces<Ok<object>>(
                StatusCodes.Status200OK
            );

        transactions.MapPost("/import", ImportTransactions)
            .WithName("ImportTransactions")
            .WithSummary("Importa transações do Oracle")
            .WithDescription("""
                Importa as transações da tabela P_TRANSACTION
                do Oracle para o MongoDB.
                """)
            .Produces(StatusCodes.Status200OK);
    }

    static async Task<
        Ok<List<ImportedTransactionResponseDto>>
    > GetAllTransactions(
        ImportedTransactionService service,
        int page = 1,
        int pageSize = 20)
    {
        var transactions = await service.GetAllAsync(
            page,
            pageSize
        );

        var response = transactions
            .Select(
                ImportedTransactionResponseDto.FromEntity
            )
            .ToList();

        return TypedResults.Ok(response);
    }

    static async Task<
        Results<
            Ok<ImportedTransactionResponseDto>,
            NotFound
        >
    > GetTransactionById(
        string id,
        ImportedTransactionService service)
    {
        var transaction =
            await service.GetByIdAsync(id);

        if (transaction is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(
            ImportedTransactionResponseDto
                .FromEntity(transaction)
        );
    }

    static async Task<
        Created<ImportedTransactionResponseDto>
    > CreateTransaction(
        ImportedTransactionDto dto,
        ImportedTransactionService service)
    {
        var transaction = new ImportedTransaction
        {
            OracleTransactionId = dto.OracleTransactionId,
            TransactionValue = dto.TransactionValue,
            Description = dto.Description,
            TransactionDate = dto.TransactionDate,
            HasOccurred = dto.HasOccurred,
            IsAutoConfirmed = dto.IsAutoConfirmed,
            Observation = dto.Observation,
            OracleCreatedAt = dto.OracleCreatedAt,
            OracleUpdatedAt = dto.OracleUpdatedAt
        };

        await service.CreateAsync(transaction);

        return TypedResults.Created(
            $"/transactions/{transaction.Id}",
            ImportedTransactionResponseDto
                .FromEntity(transaction)
        );
    }

    static async Task<
        Results<
            Ok<ImportedTransactionResponseDto>,
            NotFound
        >
    > UpdateTransaction(
        string id,
        UpdateImportedTransactionDto dto,
        ImportedTransactionService service)
    {
        var transaction =
            await service.GetByIdAsync(id);

        if (transaction is null)
            return TypedResults.NotFound();

        transaction.TransactionValue =
            dto.TransactionValue;

        transaction.Description =
            dto.Description;

        transaction.TransactionDate =
            dto.TransactionDate;

        transaction.HasOccurred =
            dto.HasOccurred;

        transaction.IsAutoConfirmed =
            dto.IsAutoConfirmed;

        transaction.Observation =
            dto.Observation;

        transaction.UpdatedAt =
            DateTime.UtcNow;

        await service.UpdateAsync(id, transaction);

        return TypedResults.Ok(
            ImportedTransactionResponseDto
                .FromEntity(transaction)
        );
    }

    static async Task<
        Results<NoContent, NotFound>
    > DeleteTransaction(
        string id,
        ImportedTransactionService service)
    {
        var deleted =
            await service.DeleteByIdAsync(id);

        if (!deleted)
            return TypedResults.NotFound();

        return TypedResults.NoContent();
    }

    static async Task<IResult> DeleteAllTransactions(
        ImportedTransactionService service)
    {
        var deletedCount =
            await service.DeleteAllAsync();

        return TypedResults.Ok(new
        {
            deletedCount
        });
    }

    static async Task<IResult> ImportTransactions(
        AppDbContext db,
        ImportedTransactionService service)
    {
        var oracleTransactions =
            await db.Transactions.ToListAsync();

        var importedCount =
            await service.ImportFromOracleAsync(
                oracleTransactions
            );

        return TypedResults.Ok(new
        {
            importedCount
        });
    }
}