using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure.Repositories;

/// <summary>
///     EF Core implementation of ITransactionRepository
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly BankDbContext _context;
    private readonly ILogger<TransactionRepository> _logger;

    public TransactionRepository(BankDbContext context, ILogger<TransactionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting transaction by ID: {TransactionId}", id);
        return await _context.Transactions.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Transaction?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting transaction by reference number: {ReferenceNumber}", referenceNumber);
        return await _context.Transactions
            .FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNumber, cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId, int limit = 100, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting transactions for account: {AccountId}", accountId);
        return await _context.Transactions
            .Where(t => t.SourceAccountId == accountId || t.TargetAccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetBySourceAccountIdAsync(Guid sourceAccountId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting transactions by source account: {SourceAccountId}", sourceAccountId);
        return await _context.Transactions
            .Where(t => t.SourceAccountId == sourceAccountId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByTargetAccountIdAsync(Guid targetAccountId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting transactions by target account: {TargetAccountId}", targetAccountId);
        return await _context.Transactions
            .Where(t => t.TargetAccountId == targetAccountId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting transactions from {FromDate} to {ToDate}", from, to);
        return await _context.Transactions
            .Where(t => t.CreatedAt >= from && t.CreatedAt <= to)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding new transaction: {TransactionId}", transaction.Id);
        await _context.Transactions.AddAsync(transaction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating transaction: {TransactionId}", transaction.Id);
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.AnyAsync(t => t.Id == id, cancellationToken);
    }
}
