using Bank.Domain.Common;
using Bank.Domain.ValueObjects;

namespace Bank.Domain.Entities;

/// <summary>
///     Account Entity - represents a bank account
///     Contains business logic for deposit, withdrawal, and balance management
/// </summary>
public class Account : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public string AccountNumber { get; private set; }
    public string Iban { get; private set; }
    public Money Balance { get; private set; }
    public AccountType Type { get; private set; }
    public AccountStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Account(
        Guid id,
        Guid customerId,
        string accountNumber,
        string iban,
        Money balance,
        AccountType type)
    {
        Id = id;
        CustomerId = customerId;
        AccountNumber = accountNumber;
        Iban = iban;
        Balance = balance;
        Type = type;
        Status = AccountStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public static Account Create(
        Guid customerId,
        string accountNumber,
        string iban,
        AccountType type = AccountType.Current,
        decimal initialBalance = 0)
    {
        if (customerId == Guid.Empty)
        {
            throw new DomainException("Customer ID is required");
        }

        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new DomainException("Account number is required");
        }

        if (string.IsNullOrWhiteSpace(iban))
        {
            throw new DomainException("IBAN is required");
        }

        return new Account(
            Guid.NewGuid(),
            customerId,
            accountNumber,
            iban,
            Money.Create(initialBalance),
            type);
    }

    /// <summary>
    ///     Deposits money into the account
    /// </summary>
    public void Deposit(Money amount)
    {
        if (amount.IsZero())
        {
            throw new DomainException("Deposit amount must be greater than zero");
        }

        if (Status != AccountStatus.Active)
        {
            throw new DomainException("Cannot deposit to an inactive account");
        }

        Balance = Balance.Add(amount);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    ///     Withdraws money from the account
    /// </summary>
    public void Withdraw(Money amount)
    {
        if (amount.IsZero())
        {
            throw new DomainException("Withdrawal amount must be greater than zero");
        }

        if (Status != AccountStatus.Active)
        {
            throw new DomainException("Cannot withdraw from an inactive account");
        }

        if (Balance.Amount < amount.Amount)
        {
            throw new DomainException("Insufficient funds");
        }

        Balance = Balance.Subtract(amount);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    ///     Transfers money to another account (internal transfer)
    /// </summary>
    public void TransferTo(Account targetAccount, Money amount)
    {
        Withdraw(amount);
        targetAccount.Deposit(amount);
    }

    public void Freeze()
    {
        Status = AccountStatus.Frozen;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unfreeze()
    {
        Status = AccountStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Close()
    {
        if (!Balance.IsZero())
        {
            throw new DomainException("Cannot close an account with remaining balance");
        }

        Status = AccountStatus.Closed;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
///     Account type enumeration
/// </summary>
public enum AccountType
{
    Current = 1,
    Savings = 2,
    Deposit = 3
}

/// <summary>
///     Account status enumeration
/// </summary>
public enum AccountStatus
{
    Active = 1,
    Frozen = 2,
    Closed = 3
}
