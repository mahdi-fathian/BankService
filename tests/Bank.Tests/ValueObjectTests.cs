using Bank.Domain.Common;
using Bank.Domain.ValueObjects;
using FluentAssertions;

namespace Bank.Tests;

/// <summary>
///     Unit tests for Value Objects
/// </summary>
public class ValueObjectTests
{
    #region Money Tests

    [Fact]
    public void Money_Create_WithPositiveAmount_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var money = Money.Create(1000, "IRR");

        // Assert
        money.Amount.Should().Be(1000);
        money.Currency.Should().Be("IRR");
    }

    [Fact]
    public void Money_Create_WithNegativeAmount_ShouldThrowDomainException()
    {
        // Act & Assert
        var action = () => Money.Create(-100);
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Money_Add_ShouldAddAmounts()
    {
        // Arrange
        var money1 = Money.Create(1000, "IRR");
        var money2 = Money.Create(500, "IRR");

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(1500);
        result.Currency.Should().Be("IRR");
    }

    [Fact]
    public void Money_Add_WithDifferentCurrencies_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = Money.Create(1000, "IRR");
        var money2 = Money.Create(500, "USD");

        // Act & Assert
        var action = () => money1.Add(money2);
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Money_Subtract_ShouldSubtractAmounts()
    {
        // Arrange
        var money1 = Money.Create(1000, "IRR");
        var money2 = Money.Create(300, "IRR");

        // Act
        var result = money1.Subtract(money2);

        // Assert
        result.Amount.Should().Be(700);
    }

    [Fact]
    public void Money_Subtract_WithInsufficientFunds_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = Money.Create(100, "IRR");
        var money2 = Money.Create(200, "IRR");

        // Act & Assert
        var action = () => money1.Subtract(money2);
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Money_IsZero_ShouldReturnTrueForZero()
    {
        // Arrange
        var zeroMoney = Money.Zero();

        // Assert
        zeroMoney.IsZero().Should().BeTrue();
    }

    #endregion

    #region Email Tests

    [Fact]
    public void Email_Create_WithValidEmail_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var email = Email.Create("test@example.com");

        // Assert
        email.Value.Should().Be("test@example.com");
    }

    [Fact]
    public void Email_Create_WithInvalidEmail_ShouldThrowDomainException()
    {
        // Act & Assert
        var action = () => Email.Create("invalid-email");
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Email_Create_ShouldNormalizeToLowercase()
    {
        // Arrange & Act
        var email = Email.Create("TEST@EXAMPLE.COM");

        // Assert
        email.Value.Should().Be("test@example.com");
    }

    #endregion

    #region NationalCode Tests

    [Theory]
    [InlineData("123")] // Too short
    [InlineData("12345678901")] // Too long
    [InlineData("123456789")] // Too short
    [InlineData("1111111111")] // All same digits (invalid by algorithm)
    [InlineData("2222222222")] // All same digits (invalid by algorithm)
    public void NationalCode_Create_WithInvalidCode_ShouldThrowDomainException(string invalidCode)
    {
        // Act & Assert
        var action = () => NationalCode.Create(invalidCode);
        action.Should().Throw<DomainException>();
    }

    #endregion

    #region PhoneNumber Tests

    [Fact]
    public void PhoneNumber_Create_WithValidMobile_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var phone = PhoneNumber.Create("09121234567");

        // Assert
        phone.Value.Should().Be("09121234567");
    }

    [Fact]
    public void PhoneNumber_Create_WithFormatting_ShouldNormalize()
    {
        // Arrange & Act - using a valid format after normalization
        var phone = PhoneNumber.Create("0912-123-4567");

        // Assert
        phone.Value.Should().Be("09121234567");
    }

    [Fact]
    public void PhoneNumber_Create_WithInvalidNumber_ShouldThrowDomainException()
    {
        // Act & Assert
        var action = () => PhoneNumber.Create("12345");
        action.Should().Throw<DomainException>();
    }

    #endregion
}
