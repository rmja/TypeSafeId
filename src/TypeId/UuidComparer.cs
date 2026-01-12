namespace TypeId;

internal class UuidComparer : IComparer<Guid>
{
    public static UuidComparer Instance { get; } = new();

    public int Compare(Guid x, Guid y)
    {
        Span<byte> xBytes = stackalloc byte[16];
        x.TryWriteBytes(xBytes, bigEndian: true, out _);

        Span<byte> yBytes = stackalloc byte[16];
        y.TryWriteBytes(yBytes, bigEndian: true, out _);

        return xBytes.SequenceCompareTo(yBytes);
    }
}
