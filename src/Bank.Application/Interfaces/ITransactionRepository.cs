using Bank.Domain.Entities;

namespace Bank.Application.Interfaces;

/// <summary>
///     Repository interface for Transaction entity
/// </summary>
public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Transaction?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId, int limit = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetBySourceAccountIdAsync(Guid sourceAccountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetByTargetAccountIdAsync(Guid targetAccountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
