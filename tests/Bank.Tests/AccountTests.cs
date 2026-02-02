using Bank.Domain.Common;
using Bank.Domain.Entities;
using Bank.Domain.ValueObjects;
using FluentAssertions;

namespace Bank.Tests;

/// <summary>
///     Unit tests for Account entity
/// </summary>
public class AccountTests
{
    [Fact]
    public void CreateAccount_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountNumber = "1234567890123456";
        var iban = "IR07123456789012345678";
        var initialBalance = 1000m;

        // Act
        var account = Account.Create(customerId, accountNumber, iban, AccountType.Current, initialBalance);

        // Assert
        account.Should().NotBeNull();
        account.CustomerId.Should().Be(customerId);
        account.AccountNumber.Should().Be(accountNumber);
        account.Iban.Should().Be(iban);
        account.Balance.Amount.Should().Be(initialBalance);
        account.Status.Should().Be(AccountStatus.Active);
    }

    [Fact]
    public void Deposit_ShouldIncreaseBalance()
    {
        // Arrange
        var account = CreateValidAccount();
        var initialBalance = account.Balance.Amount;
        var depositAmount = Money.Create(500);

        // Act
        account.Deposit(depositAmount);

        // Assert
        account.Balance.Amount.Should().Be(initialBalance + 500);
    }

    [Fact]
    public void Deposit_WithZeroAmount_ShouldThrowDomainException()
    {
        // Arrange
        var account = CreateValidAccount();

        // Act & Assert
        var action = () => account.Deposit(Money.Zero());
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Withdraw_WithSufficientBalance_ShouldDecreaseBalance()
    {
        // Arrange
        var account = CreateValidAccount(1000);
        var withdrawAmount = Money.Create(300);

        // Act
        account.Withdraw(withdrawAmount);

        // Assert
        account.Balance.Amount.Should().Be(700);
    }

    [Fact]
    public void Withdraw_WithInsufficientBalance_ShouldThrowDomainException()
    {
        // Arrange
        var account = CreateValidAccount(100);
        var withdrawAmount = Money.Create(200);

        // Act & Assert
        var action = () => account.Withdraw(withdrawAmount);
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void TransferTo_ShouldTransferMoneyBetweenAccounts()
    {
        // Arrange
        var sourceAccount = CreateValidAccount(1000);
        var targetAccount = CreateValidAccount(500);
        var transferAmount = Money.Create(300);

        // Act
        sourceAccount.TransferTo(targetAccount, transferAmount);

        // Assert
        sourceAccount.Balance.Amount.Should().Be(700);
        targetAccount.Balance.Amount.Should().Be(800);
    }

    [Fact]
    public void Freeze_ShouldSetStatusToFrozen()
    {
        // Arrange
        var account = CreateValidAccount();

        // Act
        account.Freeze();

        // Assert
        account.Status.Should().Be(AccountStatus.Frozen);
    }

    [Fact]
    public void Close_WithZeroBalance_ShouldCloseSuccessfully()
    {
        // Arrange
        var account = CreateValidAccount(0);

        // Act
        account.Close();

        // Assert
        account.Status.Should().Be(AccountStatus.Closed);
    }

    [Fact]
    public void Close_WithNonZeroBalance_ShouldThrowDomainException()
    {
        // Arrange
        var account = CreateValidAccount(100);

        // Act & Assert
        var action = () => account.Close();
        action.Should().Throw<DomainException>();
    }

    private static Account CreateValidAccount(decimal initialBalance = 0)
    {
        return Account.Create(
            Guid.NewGuid(),
            "1234567890123456",
            "IR07123456789012345678",
            AccountType.Current,
            initialBalance);
    }
}
