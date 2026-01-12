namespace TypeId;

/// <summary>
/// Specifies the TypeId prefix for an entity type.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct,
    AllowMultiple = false,
    Inherited = false
)]
public sealed class TypeIdAttribute : Attribute
{
    /// <summary>
    /// Gets the TypeId prefix for this entity type.
    /// </summary>
    public string? Prefix { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeIdAttribute"/> class.
    /// </summary>
    /// <param name="prefix">The TypeId prefix. If null, uses the decorated type name converted to lowercase.</param>
    public TypeIdAttribute(string? prefix = null)
    {
        Prefix = prefix;
    }
}
