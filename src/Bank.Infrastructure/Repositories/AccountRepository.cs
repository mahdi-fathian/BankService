using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure.Repositories;

/// <summary>
///     EF Core implementation of IAccountRepository
/// </summary>
public class AccountRepository : IAccountRepository
{
    private readonly BankDbContext _context;
    private readonly ILogger<AccountRepository> _logger;

    public AccountRepository(BankDbContext context, ILogger<AccountRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting account by ID: {AccountId}", id);
        return await _context.Accounts.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting account by account number: {AccountNumber}", accountNumber);
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, cancellationToken);
    }

    public async Task<Account?> GetByIbanAsync(string iban, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting account by IBAN: {Iban}", iban);
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Iban == iban, cancellationToken);
    }

    public async Task<IEnumerable<Account>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting accounts by customer ID: {CustomerId}", customerId);
        return await _context.Accounts
            .Where(a => a.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Account>> GetActiveAccountsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting active accounts");
        return await _context.Accounts
            .Where(a => a.Status == AccountStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding new account: {AccountId}", account.Id);
        await _context.Accounts.AddAsync(account, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating account: {AccountId}", account.Id);
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting account: {AccountId}", id);
        var account = await GetByIdAsync(id, cancellationToken);
        if (account is not null)
        {
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts.AnyAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<bool> AccountNumberExistsAsync(string accountNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .AnyAsync(a => a.AccountNumber == accountNumber, cancellationToken);
    }
}
