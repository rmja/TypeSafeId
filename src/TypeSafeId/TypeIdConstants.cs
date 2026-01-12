namespace TypeSafeId;

internal static class TypeIdConstants
{
    public const int MaxPrefixLength = 63;
    public const int IdLength = 26;
    public const int MaxLength = MaxPrefixLength + 1 + IdLength;
}
