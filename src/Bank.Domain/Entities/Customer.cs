using Bank.Domain.Common;
using Bank.Domain.ValueObjects;

namespace Bank.Domain.Entities;

/// <summary>
///     Customer Entity - represents a bank customer
///     Follows SRP: only manages customer-related data and behavior
/// </summary>
public class Customer : IAggregateRoot
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public NationalCode NationalCode { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public CustomerStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Parameterless constructor for EF Core
    private Customer()
    {
    }

    private Customer(
        Guid id,
        string firstName,
        string lastName,
        NationalCode nationalCode,
        Email email,
        PhoneNumber phoneNumber,
        DateTime dateOfBirth)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        NationalCode = nationalCode;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        Status = CustomerStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public static Customer Create(
        string firstName,
        string lastName,
        string nationalCode,
        string email,
        string phoneNumber,
        DateTime dateOfBirth)
    {
        return new Customer(
            Guid.NewGuid(),
            firstName,
            lastName,
            NationalCode.Create(nationalCode),
            Email.Create(email),
            PhoneNumber.Create(phoneNumber),
            dateOfBirth);
    }

    public void UpdateInfo(string firstName, string lastName, string email, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = Email.Create(email);
        PhoneNumber = PhoneNumber.Create(phoneNumber);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = CustomerStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = CustomerStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public string FullName => $"{FirstName} {LastName}";
}

/// <summary>
///     Customer status enumeration
/// </summary>
public enum CustomerStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3
}
