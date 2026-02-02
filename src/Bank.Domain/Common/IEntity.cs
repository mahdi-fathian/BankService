namespace Bank.Domain.Common;

/// <summary>
///     Base interface for all entities in the domain
/// </summary>
public interface IEntity
{
    Guid Id { get; }
}

/// <summary>
///     Marker interface for aggregate roots
/// </summary>
public interface IAggregateRoot : IEntity { }
