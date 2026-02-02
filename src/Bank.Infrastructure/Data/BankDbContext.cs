using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure.Data;

/// <summary>
///     Entity Framework DbContext for Bank database
///     All EF Core specific code is in Infrastructure layer (Dependency Inversion)
/// </summary>
public class BankDbContext : DbContext
{
    private readonly ILogger<BankDbContext> _logger;

    public BankDbContext(DbContextOptions<BankDbContext> options, ILogger<BankDbContext> logger)
        : base(options)
    {
        _logger = logger;
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.FirstName).HasColumnName("FirstName").HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasColumnName("LastName").HasMaxLength(50).IsRequired();
            entity.Property(e => e.DateOfBirth).HasColumnName("DateOfBirth").IsRequired();
            entity.Property(e => e.Status).HasColumnName("Status").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");

            // Value Objects as owned types
            entity.OwnsOne(e => e.NationalCode, vo =>
            {
                vo.Property(v => v.Value).HasColumnName("NationalCode").HasMaxLength(10).IsRequired();
            });

            entity.OwnsOne(e => e.Email, vo =>
            {
                vo.Property(v => v.Value).HasColumnName("Email").HasMaxLength(100).IsRequired();
            });

            entity.OwnsOne(e => e.PhoneNumber, vo =>
            {
                vo.Property(v => v.Value).HasColumnName("PhoneNumber").HasMaxLength(11).IsRequired();
            });
        });

        // Account configuration
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Accounts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerId").IsRequired();
            entity.Property(e => e.AccountNumber).HasColumnName("AccountNumber").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Iban).HasColumnName("Iban").HasMaxLength(26).IsRequired();
            entity.Property(e => e.Type).HasColumnName("Type").IsRequired();
            entity.Property(e => e.Status).HasColumnName("Status").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");

            // Value Object as owned type
            entity.OwnsOne(e => e.Balance, vo =>
            {
                vo.Property(v => v.Amount).HasColumnName("BalanceAmount").HasColumnType("decimal(18,2)").IsRequired();
                vo.Property(v => v.Currency).HasColumnName("BalanceCurrency").HasMaxLength(3).IsRequired();
            });
        });

        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transactions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.SourceAccountId).HasColumnName("SourceAccountId").IsRequired();
            entity.Property(e => e.TargetAccountId).HasColumnName("TargetAccountId");
            entity.Property(e => e.Type).HasColumnName("Type").IsRequired();
            entity.Property(e => e.Status).HasColumnName("Status").IsRequired();
            entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(500);
            entity.Property(e => e.ReferenceNumber).HasColumnName("ReferenceNumber").HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            entity.Property(e => e.CompletedAt).HasColumnName("CompletedAt");
            entity.Property(e => e.FailureReason).HasColumnName("FailureReason").HasMaxLength(500);

            // Value Object as owned type
            entity.OwnsOne(e => e.Amount, vo =>
            {
                vo.Property(v => v.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)").IsRequired();
                vo.Property(v => v.Currency).HasColumnName("Currency").HasMaxLength(3).IsRequired();
            });
        });

        _logger.LogInformation("BankDbContext model created successfully");
    }
}
