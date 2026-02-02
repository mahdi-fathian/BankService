using Bank.Application.Interfaces;
using Bank.Infrastructure.Data;
using Bank.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure;

/// <summary>
///     Dependency injection configuration for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BankDatabase");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'BankDatabase' is not configured");
        }

        // Register DbContext with SQL Server
        services.AddDbContext<BankDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

            // Log SQL queries in development
#if DEBUG
            options.LogTo(Console.WriteLine, LogLevel.Information);
            options.EnableSensitiveDataLogging();
#endif
        });

        // Register repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<BankDbContext>>();

        try
        {
            var context = scope.ServiceProvider.GetRequiredService<BankDbContext>();
            logger.LogInformation("Ensuring database is created...");

            await context.Database.EnsureCreatedAsync();

            logger.LogInformation("Database initialized successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }
}
