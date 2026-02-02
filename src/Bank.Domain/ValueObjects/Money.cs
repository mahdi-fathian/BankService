using Bank.Domain.Common;

namespace Bank.Domain.ValueObjects;

/// <summary>
///     Value Object representing an amount of money with currency
///     Addresses "Primitive Obsession" by replacing decimal with a rich domain object
/// </summary>
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency = "IRR")
    {
        if (amount < 0)
        {
            throw new DomainException("Money amount cannot be negative");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException("Currency cannot be empty");
        }

        return new Money(Math.Round(amount, 2), currency);
    }

    public static Money Zero(string currency = "IRR") => new(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new DomainException("Cannot add money with different currencies");
        }

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new DomainException("Cannot subtract money with different currencies");
        }

        if (Amount < other.Amount)
        {
            throw new DomainException("Insufficient funds");
        }

        return new Money(Amount - other.Amount, Currency);
    }

    public bool IsZero() => Amount == 0;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N0} {Currency}";

    public static implicit operator decimal(Money money) => money.Amount;
}
