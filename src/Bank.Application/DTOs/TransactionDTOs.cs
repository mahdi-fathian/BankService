using System.ComponentModel.DataAnnotations;

namespace Bank.Application.DTOs;

/// <summary>
///     Input DTO for transfer operation
/// </summary>
public class TransferInput
{
    [Required(ErrorMessage = "Source account ID is required")]
    public Guid SourceAccountId { get; set; }

    [Required(ErrorMessage = "Target account ID is required")]
    public Guid TargetAccountId { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    public string? Description { get; set; }
}

/// <summary>
///     Input DTO for transaction query
/// </summary>
public class TransactionQueryInput
{
    public Guid? AccountId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
///     Output DTO for transaction information
/// </summary>
public class TransactionOutput
{
    public Guid Id { get; set; }
    public Guid SourceAccountId { get; set; }
    public Guid? TargetAccountId { get; set; }
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? FailureReason { get; set; }
}

/// <summary>
///     Output DTO for transfer result
/// </summary>
public class TransferResultOutput
{
    public bool Success { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ErrorMessage { get; set; }
    public TransactionOutput? Transaction { get; set; }
}
