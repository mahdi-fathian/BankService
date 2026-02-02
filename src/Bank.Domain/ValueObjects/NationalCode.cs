using Bank.Domain.Common;

namespace Bank.Domain.ValueObjects;

/// <summary>
///     Value Object for Iranian National Code (کد ملی)
///     Implements the official validation algorithm for Iranian national codes
/// </summary>
public class NationalCode : ValueObject
{
    public string Value { get; }

    private NationalCode(string value)
    {
        Value = value;
    }

    public static NationalCode Create(string nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            throw new DomainException("National code cannot be empty");
        }

        var digits = new string(nationalCode.Where(char.IsDigit).ToArray());

        if (digits.Length != 10)
        {
            throw new DomainException("National code must be 10 digits");
        }

        if (!IsValidNationalCode(digits))
        {
            throw new DomainException("Invalid national code");
        }

        return new NationalCode(digits);
    }

    private static bool IsValidNationalCode(string code)
    {
        // Check for equal digits (all same digits is invalid)
        if (code.All(c => c == code[0]))
        {
            return false;
        }

        // Official validation algorithm
        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += int.Parse(code[i].ToString()) * (10 - i);
        }

        var remainder = sum % 11;
        var controlDigit = int.Parse(code[9].ToString());

        return remainder < 2 ? controlDigit == remainder : controlDigit == 11 - remainder;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
