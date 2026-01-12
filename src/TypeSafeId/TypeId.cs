using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;
using TypeSafeId.JsonConverters;

namespace TypeSafeId;

/// <summary>
/// Represents a TypeId - a type-safe, K-sortable, globally unique identifier with an optional prefix.
/// TypeIds combine a human-readable prefix with a UUIDv7 encoded in Crockford Base32.
/// </summary>
/// <remarks>
/// <para>
/// TypeIds follow the specification at https://github.com/jetify-com/typeid.
/// Format: [prefix_]<base32-encoded-uuid> where prefix is optional and lowercase.
/// </para>
/// <para>
/// Examples:
/// <list type="bullet">
/// <item><description>user_01h455vb4pex5vsknk084sn02q</description></item>
/// <item><description>order_01h455vb4pex5vsknk084sn02r</description></item>
/// <item><description>01h455vb4pex5vsknk084sn02s (no prefix)</description></item>
/// </list>
/// </para>
/// </remarks>
[JsonConverter(typeof(TypeIdJsonConverter))]
public readonly struct TypeId
    : IEquatable<TypeId>,
        ISpanParsable<TypeId>,
        IUtf8SpanParsable<TypeId>,
        ISpanFormattable,
        IComparable<TypeId>,
        IComparable
{
    private static UuidGenerator _uuidGenerator = new();
    private readonly string? _prefix;

    /// <summary>
    /// Gets the prefix component of this TypeId.
    /// Returns an empty string if no prefix is set.
    /// </summary>
    public string Prefix => _prefix ?? string.Empty;

    /// <summary>
    /// Gets the UUID component of this TypeId.
    /// </summary>
    public Guid Uuid { get; }

    /// <summary>
    /// Gets the total length of the string representation of this TypeId.
    /// </summary>
    public int Length =>
        Prefix.Length > 0 ? Prefix.Length + 1 + TypeIdConstants.IdLength : TypeIdConstants.IdLength;

    public const int MaxLength = TypeIdConstants.MaxLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeId"/> struct with the specified prefix and UUID.
    /// </summary>
    /// <param name="prefix">The prefix component. Must be lowercase letters, underscores, or empty string.</param>
    /// <param name="uuid">The UUID component.</param>
    /// <exception cref="FormatException">Thrown when the prefix is invalid.</exception>
    public TypeId(string prefix, Guid uuid)
    {
        if (Parser.ValidatePrefix(prefix) != Parser.PrefixError.None)
        {
            throw new FormatException($"Invalid TypeId prefix: '{prefix}'");
        }

        _prefix = prefix;
        Uuid = uuid;
    }

    /// <summary>
    /// Creates a new TypeId with a UUIDv7 generated from the current or specified timestamp.
    /// </summary>
    /// <param name="prefix">The prefix component.</param>
    /// <param name="timestamp">Optional timestamp to embed in the UUID. Defaults to current UTC time.</param>
    /// <returns>A new TypeId instance with a time-ordered UUID.</returns>
    public static TypeId Create(string prefix, DateTimeOffset? timestamp = null) =>
        new(prefix, _uuidGenerator.CreateVersion7((timestamp ?? DateTimeOffset.UtcNow)));

    /// <summary>
    /// Copies the string representation of this TypeId to the specified character span.
    /// </summary>
    /// <param name="buffer">The destination buffer. Must be at least <see cref="Length"/> characters.</param>
    /// <returns>The number of characters written.</returns>
    public int CopyTo(Span<char> buffer)
    {
        var written = 0;
        if (Prefix.Length > 0)
        {
            Prefix.AsSpan().CopyTo(buffer);
            buffer[Prefix.Length] = '_';
            written += Prefix.Length + 1;
        }

        written += UuidBase32.Encode(Uuid, buffer[written..]);
        return written;
    }

    /// <summary>
    /// Returns the string representation of this TypeId in the format [prefix_]&lt;base32-uuid&gt;.
    /// </summary>
    /// <returns>A string representation of this TypeId.</returns>
    public override string ToString()
    {
        var written = 0;
        Span<char> buffer = stackalloc char[TypeIdConstants.MaxLength];

        if (Prefix.Length > 0)
        {
            Prefix.AsSpan().CopyTo(buffer);
            buffer[Prefix.Length] = '_';
            written += Prefix.Length + 1;
        }

        written += UuidBase32.Encode(Uuid, buffer[written..]);
        return new string(buffer[..written]);
    }

    /// <inheritdoc/>
    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider
    )
    {
        if (destination.Length < Length)
        {
            charsWritten = 0;
            return false;
        }

        charsWritten = CopyTo(destination);
        return true;
    }

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    /// <summary>
    /// Determines whether this TypeId is equal to another TypeId.
    /// Two TypeIds are equal if they have the same prefix and UUID.
    /// </summary>
    /// <param name="other">The TypeId to compare with.</param>
    /// <returns>true if the TypeIds are equal; otherwise, false.</returns>
    public bool Equals(TypeId other) => Prefix == other.Prefix && Uuid.Equals(other.Uuid);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is TypeId other && Equals(other);

    /// <summary>
    /// Compares this TypeId with another using the default comparer.
    /// </summary>
    /// <param name="other">The TypeId to compare with.</param>
    /// <returns>A value indicating the relative order of the TypeIds.</returns>
    /// <seealso cref="TypeIdComparer"/>
    public int CompareTo(TypeId other) => TypeIdComparer.Default.Compare(this, other);

    /// <inheritdoc/>
    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }

        return obj is TypeId other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(TypeId)}");
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Prefix, Uuid);

    /// <summary>
    /// Determines whether two TypeIds are equal.
    /// </summary>
    public static bool operator ==(in TypeId left, in TypeId right) => left.Equals(right);

    /// <summary>
    /// Determines whether two TypeIds are not equal.
    /// </summary>
    public static bool operator !=(in TypeId left, in TypeId right) => !left.Equals(right);

    /// <summary>
    /// Determines whether one TypeId is less than another.
    /// </summary>
    public static bool operator <(in TypeId left, in TypeId right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Determines whether one TypeId is less than or equal to another.
    /// </summary>
    public static bool operator <=(in TypeId left, in TypeId right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Determines whether one TypeId is greater than another.
    /// </summary>
    public static bool operator >(in TypeId left, in TypeId right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Determines whether one TypeId is greater than or equal to another.
    /// </summary>
    public static bool operator >=(in TypeId left, in TypeId right) => left.CompareTo(right) >= 0;

    /// <summary>
    /// Parses a string representation of a TypeId.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">An optional format provider (not used).</param>
    /// <returns>The parsed TypeId.</returns>
    /// <exception cref="FormatException">Thrown when the string is not a valid TypeId.</exception>
    public static TypeId Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null) =>
        TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"Invalid TypeId format: '{s}'");

    /// <summary>
    /// Attempts to parse a string representation of a TypeId.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">An optional format provider (not used).</param>
    /// <param name="result">When this method returns, contains the parsed TypeId if successful; otherwise, the default value.</param>
    /// <returns>true if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out TypeId result)
    {
        if (s.Length < TypeIdConstants.IdLength)
        {
            result = default;
            return false;
        }
        else if (s.Length > TypeIdConstants.MaxLength)
        {
            result = default;
            return false;
        }
        else if (s.Length > TypeIdConstants.IdLength)
        {
            var separatorIndex = s.Length - TypeIdConstants.IdLength - 1;
            if (s[separatorIndex] != '_')
            {
                result = default;
                return false;
            }

            var prefix = s[..separatorIndex];
            var error = Parser.ValidatePrefix(prefix);
            if (error != Parser.PrefixError.None)
            {
                result = default;
                return false;
            }

            var base32 = s[(separatorIndex + 1)..];
            if (!UuidBase32.TryDecode(base32, out var uuid))
            {
                result = default;
                return false;
            }

            result = new TypeId(prefix.ToString(), uuid);
            return true;
        }
        else
        {
            if (!UuidBase32.TryDecode(s, out var uuid))
            {
                result = default;
                return false;
            }

            result = new TypeId(string.Empty, uuid);
            return true;
        }
    }

    /// <inheritdoc cref="Parse(ReadOnlySpan{char}, IFormatProvider?)"/>
    public static TypeId Parse(string s, IFormatProvider? provider) =>
        TryParse(s.AsSpan(), provider, out var result)
            ? result
            : throw new FormatException($"Invalid TypeId format: '{s}'");

    /// <inheritdoc cref="TryParse(ReadOnlySpan{char}, IFormatProvider?, out TypeId)"/>
    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out TypeId result
    ) => TryParse(s.AsSpan(), provider, out result);

    /// <summary>
    /// Parses a UTF-8 encoded string representation of a TypeId.
    /// </summary>
    /// <param name="utf8Text">The UTF-8 encoded bytes to parse.</param>
    /// <param name="provider">An optional format provider (not used).</param>
    /// <returns>The parsed TypeId.</returns>
    /// <exception cref="FormatException">Thrown when the bytes are not a valid TypeId.</exception>
    public static TypeId Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider = null) =>
        TryParse(utf8Text, provider, out var result)
            ? result
            : throw new FormatException(
                $"Invalid TypeId format: '{Encoding.UTF8.GetString(utf8Text)}'"
            );

    /// <summary>
    /// Attempts to parse a UTF-8 encoded string representation of a TypeId.
    /// </summary>
    /// <param name="utf8Text">The UTF-8 encoded bytes to parse.</param>
    /// <param name="provider">An optional format provider (not used).</param>
    /// <param name="result">When this method returns, contains the parsed TypeId if successful; otherwise, the default value.</param>
    /// <returns>true if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out TypeId result
    )
    {
        var charCount = Encoding.UTF8.GetCharCount(utf8Text);
        if (charCount != utf8Text.Length)
        {
            // Early exit if the lengths don't match, indicating invalid UTF-8 for TypeId.
            result = default;
            return false;
        }
        if (charCount > TypeIdConstants.MaxLength)
        {
            result = default;
            return false;
        }

        Span<char> charBuffer = stackalloc char[charCount];
        Encoding.UTF8.GetChars(utf8Text, charBuffer);
        return TryParse(charBuffer, provider, out result);
    }

    internal static class Parser
    {
        private static readonly SearchValues<char> Alphabet = SearchValues.Create(
            "_abcdefghijklmnopqrstuvwxyz"
        );

        public static PrefixError ValidatePrefix(ReadOnlySpan<char> prefix)
        {
            if (prefix.Length == 0)
            {
                return PrefixError.None;
            }
            if (prefix.Length > TypeIdConstants.MaxPrefixLength)
            {
                return PrefixError.TooLong;
            }
            if (prefix[0] == '_')
            {
                return PrefixError.StartsWithUnderscore;
            }
            if (prefix[^1] == '_')
            {
                return PrefixError.EndsWithUnderscore;
            }

            return prefix.ContainsAnyExcept(Alphabet) ? PrefixError.InvalidChar : PrefixError.None;
        }

        public enum PrefixError
        {
            None,
            TooLong,
            StartsWithUnderscore,
            EndsWithUnderscore,
            InvalidChar,
        }
    }
}
