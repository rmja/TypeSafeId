namespace TypeSafeId;

/// <summary>
/// Provides comparers for ordering and comparing TypeId instances using different criteria.
/// </summary>
/// <remarks>Use the comparers exposed by this class to perform lexicographical or timestamp-based comparisons of
/// TypeId values. The Default property determines the comparer used by default throughout the application. Changing the
/// Default comparer affects all operations that rely on it. Reading the Default property is thread-safe; however,
/// setting it should be done during application initialization to avoid race conditions.</remarks>
public static class TypeIdComparer
{
    /// <summary>
    /// Gets a comparer that performs a lexicographical comparison of TypeId instances.
    /// </summary>
    public static IComparer<TypeId> Lex => TypeIdLexComparer.Instance;

    /// <summary>
    /// Gets a comparer that orders TypeId instances by their timestamp value.
    /// </summary>
    /// <remarks>Use this comparer to sort or compare TypeId objects based on the chronological order of their
    /// associated timestamps. This is useful when processing collections where temporal ordering is required.</remarks>
    public static IComparer<TypeId> Timestamp => TypeIdTimestampComparer.Instance;

    /// <summary>
    /// Gets or sets the default comparer used to compare <see cref="TypeId"/> instances.
    /// </summary>
    /// <remarks>The default comparer determines how <see cref="TypeId"/> values are ordered and compared
    /// throughout the application. Changing this property affects all operations that rely on the default comparer.
    /// This property is thread-safe for reading, but setting it is not thread-safe and should be performed during
    /// application initialization.</remarks>
    public static IComparer<TypeId> Default { get; set; } = Lex;
}

class TypeIdLexComparer : IComparer<TypeId>
{
    public static TypeIdLexComparer Instance { get; } = new();

    public int Compare(TypeId x, TypeId y)
    {
        var prefixComparison = string.CompareOrdinal(x.Prefix, y.Prefix);
        if (prefixComparison != 0)
        {
            return prefixComparison;
        }

        return UuidComparer.Instance.Compare(x.Uuid, y.Uuid);
    }
}

class TypeIdTimestampComparer : IComparer<TypeId>
{
    public static TypeIdTimestampComparer Instance { get; } = new();

    public int Compare(TypeId x, TypeId y)
    {
        var uuidComparison = UuidComparer.Instance.Compare(x.Uuid, y.Uuid);
        if (uuidComparison != 0)
        {
            return uuidComparison;
        }

        return string.CompareOrdinal(x.Prefix, y.Prefix);
    }
}
