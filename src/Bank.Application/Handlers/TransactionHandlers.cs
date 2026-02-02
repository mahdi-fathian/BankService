using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Common;
using Bank.Domain.Entities;
using Bank.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bank.Application.Handlers;

/// <summary>
///     Request for transfer operation
/// </summary>
public class TransferRequest : IRequest<TransferResultOutput>
{
    public Guid SourceAccountId { get; set; }
    public Guid TargetAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}

/// <summary>
///     Handler for transfer operation
/// </summary>
public class TransferHandler : IRequestHandler<TransferRequest, TransferResultOutput>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<TransferHandler> _logger;

    public TransferHandler(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        ILogger<TransferHandler> logger)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<TransferResultOutput> Handle(TransferRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing transfer from {SourceAccountId} to {TargetAccountId}",
            request.SourceAccountId,
            request.TargetAccountId);

        var result = new TransferResultOutput { Success = false };

        var sourceAccount = await _accountRepository.GetByIdAsync(request.SourceAccountId, cancellationToken);
        if (sourceAccount is null)
        {
            result.ErrorMessage = "Source account not found";
            return result;
        }

        var targetAccount = await _accountRepository.GetByIdAsync(request.TargetAccountId, cancellationToken);
        if (targetAccount is null)
        {
            result.ErrorMessage = "Target account not found";
            return result;
        }

        var amount = Money.Create(request.Amount);
        var description = request.Description ?? "Transfer";

        var transaction = Transaction.CreateTransfer(sourceAccount.Id, targetAccount.Id, amount, description);

        try
        {
            sourceAccount.TransferTo(targetAccount, amount);
            transaction.MarkAsCompleted();

            await _accountRepository.UpdateAsync(sourceAccount, cancellationToken);
            await _accountRepository.UpdateAsync(targetAccount, cancellationToken);
        }
        catch (DomainException ex)
        {
            transaction.MarkAsFailed(ex.Message);
        }

        await _transactionRepository.AddAsync(transaction, cancellationToken);

        result.Transaction = MapToOutput(transaction);

        if (transaction.Status == TransactionStatus.Completed)
        {
            result.Success = true;
            result.ReferenceNumber = transaction.ReferenceNumber;
            _logger.LogInformation("Transfer completed successfully. Reference: {ReferenceNumber}", transaction.ReferenceNumber);
        }
        else
        {
            result.ErrorMessage = transaction.FailureReason;
            _logger.LogWarning("Transfer failed: {ErrorMessage}", transaction.FailureReason);
        }

        return result;
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
///     Request for getting account balance
/// </summary>
public class GetBalanceRequest : IRequest<BalanceOutput?>
{
    public Guid AccountId { get; set; }
}

/// <summary>
///     Handler for getting account balance
/// </summary>
public class GetBalanceHandler : IRequestHandler<GetBalanceRequest, BalanceOutput?>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<GetBalanceHandler> _logger;

    public GetBalanceHandler(
        IAccountRepository accountRepository,
        ILogger<GetBalanceHandler> logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task<BalanceOutput?> Handle(GetBalanceRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting balance for account: {AccountId}", request.AccountId);

        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null)
        {
            return null;
        }

        return new BalanceOutput
        {
            AccountId = account.Id,
            AccountNumber = account.AccountNumber,
            Amount = account.Balance.Amount,
            Currency = account.Balance.Currency,
            AsOfDate = DateTime.UtcNow
        };
    }
}
