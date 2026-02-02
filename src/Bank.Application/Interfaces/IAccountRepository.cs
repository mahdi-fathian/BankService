using Bank.Domain.Entities;

namespace Bank.Application.Interfaces;

/// <summary>
///     Repository interface for Account entity
/// </summary>
public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task<Account?> GetByIbanAsync(string iban, CancellationToken cancellationToken = default);
    Task<IEnumerable<Account>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Account>> GetActiveAccountsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Account account, CancellationToken cancellationToken = default);
    Task UpdateAsync(Account account, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AccountNumberExistsAsync(string accountNumber, CancellationToken cancellationToken = default);
}
