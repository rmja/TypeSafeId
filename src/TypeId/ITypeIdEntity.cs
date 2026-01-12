namespace TypeId;

/// <summary>
/// Defines an entity that can be identified by a TypeId with a specific prefix.
/// </summary>
public interface ITypeIdEntity
{
    /// <summary>
    /// Gets the TypeId prefix for this entity type.
    /// </summary>
    static abstract string TypeIdPrefix { get; }
}
