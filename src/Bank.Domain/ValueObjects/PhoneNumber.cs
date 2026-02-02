using Bank.Domain.Common;

namespace Bank.Domain.ValueObjects;

/// <summary>
///     Value Object for Phone Number (Iran mobile numbers)
/// </summary>
public class PhoneNumber : ValueObject
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new DomainException("Phone number cannot be empty");
        }

        // Remove all non-digit characters
        var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

        // Iranian mobile numbers should start with 09 and be 11 digits
        if (digits.Length == 11 && digits.StartsWith("09"))
        {
            return new PhoneNumber(digits);
        }

        // Allow landline numbers with area code (10-11 digits)
        if (digits.Length >= 10 && digits.Length <= 11)
        {
            return new PhoneNumber(digits);
        }

        throw new DomainException("Invalid phone number format");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
