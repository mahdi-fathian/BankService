using Bank.Domain.Common;
using Bank.Domain.ValueObjects;

namespace Bank.Domain.Entities;

/// <summary>
///     Transaction Entity - represents a banking transaction
///     Contains all transaction-related data and behavior
/// </summary>
public class Transaction : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid SourceAccountId { get; private set; }
    public Guid? TargetAccountId { get; private set; }
    public Money Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string Description { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? FailureReason { get; private set; }

    // Parameterless constructor for EF Core
    private Transaction()
    {
    }

    private Transaction(
        Guid id,
        Guid sourceAccountId,
        Guid? targetAccountId,
        Money amount,
        TransactionType type,
        string description)
    {
        Id = id;
        SourceAccountId = sourceAccountId;
        TargetAccountId = targetAccountId;
        Amount = amount;
        Type = type;
        Status = TransactionStatus.Pending;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    public static Transaction CreateDeposit(
        Guid accountId,
        Money amount,
        string description = "Deposit")
    {
        if (amount.IsZero())
        {
            throw new DomainException("Transaction amount must be greater than zero");
        }

        return new Transaction(
            Guid.NewGuid(),
            accountId,
            null,
            amount,
            TransactionType.Deposit,
            description);
    }

    public static Transaction CreateWithdrawal(
        Guid accountId,
        Money amount,
        string description = "Withdrawal")
    {
        if (amount.IsZero())
        {
            throw new DomainException("Transaction amount must be greater than zero");
        }

        return new Transaction(
            Guid.NewGuid(),
            accountId,
            null,
            amount,
            TransactionType.Withdrawal,
            description);
    }

    public static Transaction CreateTransfer(
        Guid sourceAccountId,
        Guid targetAccountId,
        Money amount,
        string description = "Transfer")
    {
        if (amount.IsZero())
        {
            throw new DomainException("Transaction amount must be greater than zero");
        }

        if (sourceAccountId == targetAccountId)
        {
            throw new DomainException("Source and target accounts cannot be the same");
        }

        var transaction = new Transaction(
            Guid.NewGuid(),
            sourceAccountId,
            targetAccountId,
            amount,
            TransactionType.Transfer,
            description);

        transaction.ReferenceNumber = GenerateReferenceNumber();
        return transaction;
    }

    public void MarkAsCompleted()
    {
        Status = TransactionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string reason)
    {
        Status = TransactionStatus.Failed;
        FailureReason = reason;
        CompletedAt = DateTime.UtcNow;
    }

    public void MarkAsCancelled(string reason = "Cancelled by user")
    {
        Status = TransactionStatus.Cancelled;
        FailureReason = reason;
        CompletedAt = DateTime.UtcNow;
    }

    private static string GenerateReferenceNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"TRX{timestamp}{random}";
    }
}

/// <summary>
///     Transaction type enumeration
/// </summary>
public enum TransactionType
{
    Deposit = 1,
    Withdrawal = 2,
    Transfer = 3
}

/// <summary>
///     Transaction status enumeration
/// </summary>
public enum TransactionStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4
}
