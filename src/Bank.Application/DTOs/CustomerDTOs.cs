using System.ComponentModel.DataAnnotations;

namespace Bank.Application.DTOs;

/// <summary>
///     Input DTO for creating a new customer
/// </summary>
public class CreateCustomerInput
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "National code is required")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "National code must be 10 digits")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "National code must be 10 digits")]
    public string NationalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^09\d{9}$", ErrorMessage = "Invalid Iranian mobile number format")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }
}

/// <summary>
///     Input DTO for updating customer information
/// </summary>
public class UpdateCustomerInput
{
    [Required(ErrorMessage = "Customer ID is required")]
    public Guid CustomerId { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    public string? FirstName { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
    public string? LastName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [RegularExpression(@"^09\d{9}$", ErrorMessage = "Invalid Iranian mobile number format")]
    public string? PhoneNumber { get; set; }
}

/// <summary>
///     Output DTO for customer information (separate from Input)
/// </summary>
public class CustomerOutput
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
///     Input DTO for customer query parameters
/// </summary>
public class CustomerQueryInput
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
