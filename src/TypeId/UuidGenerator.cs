using System.Buffers.Binary;
using System.Diagnostics;
using System.Security.Cryptography;

namespace TypeId;

/// <summary>
/// Generator for UUIDv7 identifiers according to RFC 9562.
/// </summary>
/// <remarks>
/// This implementation provides enhanced sub-millisecond ordering by embedding
/// timestamp fractions (ticks) in the random section, as permitted by RFC 9562 Section 6.2.
/// </remarks>
internal sealed class UuidGenerator
{
    private const int SequenceBitSize = 7;
    private const int SequenceMaxValue = (1 << SequenceBitSize) - 1;

    /// <summary>
    /// Creates a UUIDv7 (RFC 9562) with the specified timestamp.
    /// </summary>
    /// <param name="timestamp">The timestamp to embed in the UUID. Must not be negative (before Unix epoch).</param>
    /// <returns>A UUIDv7 with the following structure:
    /// <list type="bullet">
    /// <item><description><b>unix_ts_ms</b> (48 bits): Unix timestamp in milliseconds</description></item>
    /// <item><description><b>ver</b> (4 bits): Version 7 (0b0111)</description></item>
    /// <item><description><b>rand_a</b> (12 bits): Sub-millisecond timestamp fraction (upper 12 bits of ticks remainder)</description></item>
    /// <item><description><b>var</b> (2 bits): Variant (0b10)</description></item>
    /// <item><description><b>rand_b</b> (62 bits): Lower 2 bits of ticks remainder + 60 bits of cryptographic random data</description></item>
    /// </list>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="timestamp"/> is before the Unix epoch (1970-01-01 00:00:00 UTC).</exception>
    /// <remarks>
    /// <para>
    /// This implementation enhances the standard UUIDv7 specification by using sub-millisecond
    /// precision from .NET ticks (14 bits, ~0.1ms resolution) instead of pure random data
    /// for the first 14 bits of the random section. This provides:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Better ordering guarantees for UUIDs generated within the same millisecond</description></item>
    /// <item><description>Maintained cryptographic randomness in the remaining 60 bits</description></item>
    /// <item><description>Full compliance with RFC 9562 Section 6.2, which permits sub-millisecond fractions as additional entropy</description></item>
    /// </list>
    /// <para>
    /// The sub-millisecond ticks are calculated as: <c>(timestamp.Ticks % 10000)</c>,
    /// representing the fractional milliseconds with ~100 nanosecond precision (14 bits range: 0-9999).
    /// </para>
    /// </remarks>
    public Guid CreateVersion7(DateTimeOffset timestamp)
    {
        // Allocating 2 bytes more to prepend timestamp data.
        Span<byte> buffer = stackalloc byte[18];

        // Offset bytes that are used in ID.
        var bytes = buffer[2..];

        var unixMs = timestamp.ToUnixTimeMilliseconds();
        var unixTicks = (timestamp - DateTimeOffset.UnixEpoch).Ticks;
        ArgumentOutOfRangeException.ThrowIfNegative(unixMs, nameof(timestamp));

        var reminder = unixTicks - unixMs * TimeSpan.TicksPerMillisecond;
        Debug.Assert(reminder >= 0 && reminder < TimeSpan.TicksPerMillisecond); // 14 bits

        // unix_ts_ms
        BinaryPrimitives.TryWriteInt64BigEndian(buffer[..8], unixMs); // Using full buffer because we need to account for two zero-bytes in front.

        // rand_a
        BinaryPrimitives.TryWriteUInt16BigEndian(bytes[6..8], (ushort)(reminder >> 2)); // 12 most significat bits

        // ver
        bytes[6] |= 0b0111_0000; // Set 4 upper bits to version (0b0111)

        // rand_b
        RandomNumberGenerator.Fill(bytes[8..]);
        bytes[8] &= 0b11001111; // Erase upper 2 bits
        bytes[8] |= (byte)((reminder & 0x3) << 4); // Set 2 least significant bits from reminder

        // var
        bytes[8] &= 0b0011_1111; // Erase upper 2 bits
        bytes[8] |= 0b1000_0000; // Set 2 upper bits to variant (0b10)

        return new Guid(bytes, bigEndian: true);
    }
}
