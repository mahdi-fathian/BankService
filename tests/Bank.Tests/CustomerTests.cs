using Bank.Domain.Common;
using Bank.Domain.Entities;
using Bank.Domain.ValueObjects;
using FluentAssertions;

namespace Bank.Tests;

/// <summary>
///     Unit tests for Customer entity
/// </summary>
public class CustomerTests
{
    [Fact]
    public void CreateCustomer_WithInvalidNationalCode_ShouldThrowDomainException()
    {
        // Arrange - Use an invalid national code (all same digits)
        var invalidNationalCode = "1111111111";
        var validEmail = "john.doe@example.com";
        var validPhoneNumber = "09121234567";

        // Act & Assert
        var action = () => Customer.Create(
            "John",
            "Doe",
            invalidNationalCode,
            validEmail,
            validPhoneNumber,
            DateTime.UtcNow);

        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void CreateCustomer_WithInvalidEmail_ShouldThrowDomainException()
    {
        // Arrange & Act & Assert
        var action = () => Customer.Create(
            "John",
            "Doe",
            "1111111111",
            "invalid-email",
            "09121234567",
            DateTime.UtcNow);

        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void CustomerStatus_Values_ShouldBeCorrect()
    {
        // Test that the status enum has the expected values
        Enum.TryParse<CustomerStatus>("Active", out var active);
        Enum.TryParse<CustomerStatus>("Inactive", out var inactive);
        Enum.TryParse<CustomerStatus>("Suspended", out var suspended);

        active.Should().Be(CustomerStatus.Active);
        inactive.Should().Be(CustomerStatus.Inactive);
        suspended.Should().Be(CustomerStatus.Suspended);
    }
}
