using Bank.Domain.Common;

namespace Bank.Domain.ValueObjects;

/// <summary>
///     Value Object for Email address
///     Prevents primitive obsession and ensures valid email format
/// </summary>
public class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email cannot be empty");
        }

        email = email.Trim().ToLowerInvariant();

        if (!IsValidEmail(email))
        {
            throw new DomainException("Invalid email format");
        }

        return new Email(email);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
