namespace Bank.Domain.Common;

/// <summary>
///     Base exception for all domain-related errors
///     Follows the pattern of explicit domain exceptions instead of swallowing exceptions
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
