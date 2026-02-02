using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure.Repositories;

/// <summary>
///     EF Core implementation of ICustomerRepository
///     Implements the interface defined in Application layer
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly BankDbContext _context;
    private readonly ILogger<CustomerRepository> _logger;

    public CustomerRepository(BankDbContext context, ILogger<CustomerRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting customer by ID: {CustomerId}", id);
        return await _context.Customers.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Customer?> GetByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting customer by national code: {NationalCode}", nationalCode);
        return await _context.Customers
            .FirstOrDefaultAsync(c => EF.Property<string>(c, "NationalCode") == nationalCode, cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting customer by email: {Email}", email);
        return await _context.Customers
            .FirstOrDefaultAsync(c => EF.Property<string>(c, "Email") == email, cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all customers");
        return await _context.Customers.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting active customers");
        return await _context.Customers
            .Where(c => c.Status == CustomerStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding new customer: {CustomerId}", customer.Id);
        await _context.Customers.AddAsync(customer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating customer: {CustomerId}", customer.Id);
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting customer: {CustomerId}", id);
        var customer = await GetByIdAsync(id, cancellationToken);
        if (customer is not null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> NationalCodeExistsAsync(string nationalCode, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AnyAsync(c => EF.Property<string>(c, "NationalCode") == nationalCode, cancellationToken);
    }
}
