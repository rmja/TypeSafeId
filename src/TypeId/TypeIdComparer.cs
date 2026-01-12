namespace TypeId;

public static class TypeIdComparer
{
    public static IComparer<TypeId> Lex => TypeIdLexComparer.Instance;

    public static IComparer<TypeId> Timestamp => TypeIdTimestampComparer.Instance;

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
