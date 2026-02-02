using System.ComponentModel.DataAnnotations;

namespace Bank.Application.DTOs;

/// <summary>
///     Input DTO for creating a new account
/// </summary>
public class CreateAccountInput
{
    [Required(ErrorMessage = "Customer ID is required")]
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "Account type is required")]
    public string AccountType { get; set; } = "Current";

    [Range(0, double.MaxValue, ErrorMessage = "Initial balance cannot be negative")]
    public decimal InitialBalance { get; set; } = 0;
}

/// <summary>
///     Input DTO for deposit operation
/// </summary>
public class DepositInput
{
    [Required(ErrorMessage = "Account ID is required")]
    public Guid AccountId { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    public string? Description { get; set; }
}

/// <summary>
///     Input DTO for withdrawal operation
/// </summary>
public class WithdrawInput
{
    [Required(ErrorMessage = "Account ID is required")]
    public Guid AccountId { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    public string? Description { get; set; }
}

/// <summary>
///     Input DTO for account query
/// </summary>
public class AccountQueryInput
{
    public Guid? CustomerId { get; set; }
    public string? AccountNumber { get; set; }
    public string? Iban { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
///     Output DTO for account information
/// </summary>
public class AccountOutput
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string Balance { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
///     Output DTO for balance information
/// </summary>
public class BalanceOutput
{
    public Guid AccountId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime AsOfDate { get; set; }
}
