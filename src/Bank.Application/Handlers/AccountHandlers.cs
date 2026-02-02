using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Common;
using Bank.Domain.Entities;
using Bank.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bank.Application.Handlers;

/// <summary>
///     Request for creating a new account
/// </summary>
public class CreateAccountRequest : IRequest<AccountOutput>
{
    public Guid CustomerId { get; set; }
    public string AccountType { get; set; } = "Current";
    public decimal InitialBalance { get; set; }
}

/// <summary>
///     Handler for creating a new account
/// </summary>
public class CreateAccountHandler : IRequestHandler<CreateAccountRequest, AccountOutput>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CreateAccountHandler> _logger;

    public CreateAccountHandler(
        IAccountRepository accountRepository,
        ICustomerRepository customerRepository,
        ILogger<CreateAccountHandler> logger)
    {
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<AccountOutput> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new account for customer: {CustomerId}", request.CustomerId);

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            throw new DomainException("Customer not found");
        }

        if (!Enum.TryParse<AccountType>(request.AccountType, true, out var accountType))
        {
            accountType = AccountType.Current;
        }

        var accountNumber = GenerateAccountNumber();
        var iban = GenerateIban(accountNumber);

        var account = Account.Create(
            request.CustomerId,
            accountNumber,
            iban,
            accountType,
            request.InitialBalance);

        await _accountRepository.AddAsync(account, cancellationToken);

        _logger.LogInformation("Account created successfully: {AccountId}", account.Id);

        return MapToOutput(account);
    }

    private static string GenerateAccountNumber()
    {
        var random = new Random();
        return string.Join("", Enumerable.Range(0, 16).Select(_ => random.Next(0, 10)));
    }

    private static string GenerateIban(string accountNumber)
    {
        return $"IR07{accountNumber}";
    }

    private static AccountOutput MapToOutput(Account account)
    {
        return new AccountOutput
        {
            Id = account.Id,
            CustomerId = account.CustomerId,
            AccountNumber = account.AccountNumber,
            Iban = account.Iban,
            Balance = account.Balance.Amount.ToString("N0"),
            Currency = account.Balance.Currency,
            Type = account.Type.ToString(),
            Status = account.Status.ToString(),
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };
    }
}

/// <summary>
///     Request for deposit operation
/// </summary>
public class DepositRequest : IRequest<TransactionOutput>
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}

/// <summary>
///     Handler for deposit operation
/// </summary>
public class DepositHandler : IRequestHandler<DepositRequest, TransactionOutput>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<DepositHandler> _logger;

    public DepositHandler(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        ILogger<DepositHandler> logger)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<TransactionOutput> Handle(DepositRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing deposit for account: {AccountId}", request.AccountId);

        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null)
        {
            throw new DomainException("Account not found");
        }

        var amount = Money.Create(request.Amount);
        var description = request.Description ?? "Deposit";

        var transaction = Transaction.CreateDeposit(account.Id, amount, description);
        transaction.MarkAsCompleted();

        account.Deposit(amount);
        await _accountRepository.UpdateAsync(account, cancellationToken);
        await _transactionRepository.AddAsync(transaction, cancellationToken);

        _logger.LogInformation("Deposit completed successfully. Reference: {ReferenceNumber}", transaction.ReferenceNumber);

        return MapToOutput(transaction);
    }

    private static TransactionOutput MapToOutput(Transaction transaction)
    {
        return new TransactionOutput
        {
            Id = transaction.Id,
            SourceAccountId = transaction.SourceAccountId,
            TargetAccountId = transaction.TargetAccountId,
            Amount = transaction.Amount.Amount.ToString("N0"),
            Currency = transaction.Amount.Currency,
            Type = transaction.Type.ToString(),
            Status = transaction.Status.ToString(),
            Description = transaction.Description,
            ReferenceNumber = transaction.ReferenceNumber,
            CreatedAt = transaction.CreatedAt,
            CompletedAt = transaction.CompletedAt,
            FailureReason = transaction.FailureReason
        };
    }
}

/// <summary>
///     Request for withdrawal operation
/// </summary>
public class WithdrawRequest : IRequest<TransactionOutput>
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}

/// <summary>
///     Handler for withdrawal operation
/// </summary>
public class WithdrawHandler : IRequestHandler<WithdrawRequest, TransactionOutput>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<WithdrawHandler> _logger;

    public WithdrawHandler(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        ILogger<WithdrawHandler> logger)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<TransactionOutput> Handle(WithdrawRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing withdrawal for account: {AccountId}", request.AccountId);

        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null)
        {
            throw new DomainException("Account not found");
        }

        var amount = Money.Create(request.Amount);
        var description = request.Description ?? "Withdrawal";

        var transaction = Transaction.CreateWithdrawal(account.Id, amount, description);

        try
        {
            account.Withdraw(amount);
            transaction.MarkAsCompleted();
            await _accountRepository.UpdateAsync(account, cancellationToken);
        }
        catch (DomainException ex)
        {
            transaction.MarkAsFailed(ex.Message);
        }

        await _transactionRepository.AddAsync(transaction, cancellationToken);

        if (transaction.Status == TransactionStatus.Failed)
        {
            throw new DomainException(transaction.FailureReason!);
        }

        _logger.LogInformation("Withdrawal completed successfully. Reference: {ReferenceNumber}", transaction.ReferenceNumber);

        return MapToOutput(transaction);
    }

    private static TransactionOutput MapToOutput(Transaction transaction)
    {
        return new TransactionOutput
        {
            Id = transaction.Id,
            SourceAccountId = transaction.SourceAccountId,
            TargetAccountId = transaction.TargetAccountId,
            Amount = transaction.Amount.Amount.ToString("N0"),
            Currency = transaction.Amount.Currency,
            Type = transaction.Type.ToString(),
            Status = transaction.Status.ToString(),
            Description = transaction.Description,
            ReferenceNumber = transaction.ReferenceNumber,
            CreatedAt = transaction.CreatedAt,
            CompletedAt = transaction.CompletedAt,
            FailureReason = transaction.FailureReason
        };
    }
}
