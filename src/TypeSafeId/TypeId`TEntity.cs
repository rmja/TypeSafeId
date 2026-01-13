using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace TypeSafeId;

/// <summary>
/// Represents a strongly-typed TypeId that is constrained to a specific entity type.
/// </summary>
/// <typeparam name="TEntity">The entity type that defines the TypeId prefix.</typeparam>
public readonly struct TypeId<TEntity>
    : IEquatable<TypeId<TEntity>>,
        ISpanParsable<TypeId<TEntity>>,
        IUtf8SpanParsable<TypeId<TEntity>>,
        ISpanFormattable,
        IComparable<TypeId<TEntity>>,
        IComparable
{
    private static string _prefix = GetPrefixFromAttribute();
    private readonly TypeId _value;

    /// <summary>
    /// Gets the prefix string defined by the associated attribute.
    /// </summary>
    /// <remarks>The value is determined at runtime by evaluating the attribute applied to the containing type
    /// or member.</remarks>
    public static string Prefix => _prefix;

    /// <summary>
    /// Gets the UUID component of this TypeId.
    /// </summary>
    public Guid Uuid => _value.Uuid;

    /// <summary>
    /// Gets the total length of the string representation.
    /// </summary>
    public int Length => _value.Length;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeId{TEntity}"/> struct from a UUID.
    /// </summary>
    /// <param name="uuid">The UUID component.</param>
    public TypeId(Guid uuid)
    {
        _value = new TypeId(_prefix, uuid, skipValidation: true);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeId{TEntity}"/> struct from an untyped TypeId.
    /// </summary>
    /// <param name="value">The TypeId value.</param>
    /// <exception cref="ArgumentException">Thrown when the prefix doesn't match the entity type.</exception>
    public TypeId(TypeId value)
    {
        if (value.Prefix != _prefix)
        {
            throw new ArgumentException(
                $"TypeId prefix must be '{_prefix}', got '{value.Prefix}'",
                nameof(value)
            );
        }
        _value = value;
    }

    /// <summary>
    /// Creates a new TypeId with the current timestamp.
    /// </summary>
    /// <param name="timestamp">Optional timestamp to use. Defaults to current UTC time.</param>
    /// <returns>A new TypeId instance.</returns>
    public static TypeId<TEntity> Create(DateTimeOffset? timestamp = null) =>
        new(UuidGenerator.Instance.CreateVersion7((timestamp ?? DateTimeOffset.UtcNow)));

    /// <summary>
    /// Sets the internal prefix used for type identifier parsing. Intended for testing and benchmarking scenarios only.
    /// </summary>
    /// <remarks>This method should not be used in production code. It is provided solely for
    /// testing and benchmarking purposes. Changing the prefix may affect how type identifiers are parsed and
    /// recognized.</remarks>
    /// <param name="prefix">The prefix string to use for type identifier parsing.</param>
    [Obsolete("For testing and benchmarking purposes only.")]
    public static void SetPrefix(string prefix)
    {
        if (!TypeId.Parser.IsValidPrefix(prefix))
        {
            throw new ArgumentException(
                $"TypeId prefix for entity type '{typeof(TEntity).Name}' is invalid."
            );
        }

        _prefix = prefix;
    }

    private static string GetPrefixFromAttribute()
    {
        var attribute = typeof(TEntity).GetCustomAttribute<TypeIdAttribute>();
        var prefix = attribute?.Prefix;

        // If no attribute or null prefix, use the type name as default
        prefix ??= JsonNamingPolicy.SnakeCaseLower.ConvertName(typeof(TEntity).Name);

        if (!TypeId.Parser.IsValidPrefix(prefix))
        {
            throw new InvalidOperationException(
                $"TypeId prefix for entity type '{typeof(TEntity).Name}' is invalid."
            );
        }

        return prefix;
    }

    /// <summary>
    /// Returns a new instance of <see cref="TypeId{TResult}"/> that represents the same underlying value as the current
    /// instance, but with a different type parameter.
    /// </summary>
    /// <typeparam name="TResult">The type to cast the identifier to. Must be compatible with the underlying value type.</typeparam>
    /// <returns>A <see cref="TypeId{TResult}"/> instance containing the same value as the current identifier.</returns>
    public TypeId<TResult> Cast<TResult>() => new(_value);

    /// <summary>
    /// Copies the string representation to a character span.
    /// </summary>
    public int CopyTo(Span<char> buffer) => _value.CopyTo(buffer);

    /// <summary>
    /// Returns the base-32 encoded suffix for the current UUID value.
    /// </summary>
    /// <returns>A string containing the base-32 encoded representation of the UUID suffix.</returns>
    public string GetSuffix() => _value.GetSuffix();

    /// <summary>
    /// Gets the base-32 encoded suffix for the current UUID value into the provided buffer.
    /// </summary>
    /// <param name="buffer">The buffer to where the suffix is written.</param>
    /// <returns>The number of written suffix bytes.</returns>
    public int GetSuffix(Span<char> buffer) => _value.GetSuffix(buffer);

    /// <summary>
    /// Returns the string representation of this TypeId in the format [prefix_]&lt;base32-uuid&gt;.
    /// </summary>
    /// <returns>A string representation of this TypeId.</returns>
    public override string ToString() => _value.ToString();

    /// <inheritdoc/>
    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider
    ) => _value.TryFormat(destination, out charsWritten, format, provider);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        _value.ToString(format, formatProvider);

    /// <inheritdoc/>
    public bool Equals(TypeId<TEntity> other) => _value.Equals(other._value);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        (obj is TypeId<TEntity> other1 && Equals(other1))
        || (obj is TypeId other2 && _value.Equals(other2));

    /// <inheritdoc/>
    public int CompareTo(TypeId<TEntity> other) => _value.CompareTo(other._value);

    /// <inheritdoc/>
    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }

        return obj is TypeId<TEntity> other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {typeof(TypeId<TEntity>).Name}");
    }

    /// <inheritdoc/>
    public override int GetHashCode() => _value.Uuid.GetHashCode();

    /// <summary>
    /// Implicitly converts a strongly-typed TypeId to an untyped TypeId.
    /// </summary>
    public static implicit operator TypeId(TypeId<TEntity> typed) => typed._value;

    /// <summary>
    /// Explicitly converts an untyped TypeId to a strongly-typed TypeId.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the prefix doesn't match.</exception>
    public static explicit operator TypeId<TEntity>(TypeId value) => new(value);

    /// <inheritdoc/>
    public static bool operator ==(TypeId<TEntity> left, TypeId<TEntity> right) =>
        left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(TypeId<TEntity> left, TypeId<TEntity> right) =>
        !left.Equals(right);

    /// <inheritdoc/>
    public static bool operator <(TypeId<TEntity> left, TypeId<TEntity> right) =>
        left.CompareTo(right) < 0;

    /// <inheritdoc/>
    public static bool operator <=(TypeId<TEntity> left, TypeId<TEntity> right) =>
        left.CompareTo(right) <= 0;

    /// <inheritdoc/>
    public static bool operator >(TypeId<TEntity> left, TypeId<TEntity> right) =>
        left.CompareTo(right) > 0;

    /// <inheritdoc/>
    public static bool operator >=(TypeId<TEntity> left, TypeId<TEntity> right) =>
        left.CompareTo(right) >= 0;

    /// <inheritdoc/>
    public static TypeId<TEntity> Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null) =>
        TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"Invalid TypeId<{typeof(TEntity).Name}> format: '{s}'");

    /// <inheritdoc cref="TryParse(ReadOnlySpan{char}, IFormatProvider?, out TypeId{TEntity})"/>
    public static bool TryParse(ReadOnlySpan<char> s, out TypeId<TEntity> result) =>
        TryParse(s, null, out result);

    /// <inheritdoc/>
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out TypeId<TEntity> result
    )
    {
        if (_prefix.Length > 0 && s.Length == _prefix.Length + 1 + TypeIdConstants.IdLength)
        {
            var separatorIndex = _prefix.Length;
            if (
                s[..separatorIndex].SequenceEqual(_prefix)
                && s[separatorIndex] == '_'
                && UuidBase32.TryDecode(s[(separatorIndex + 1)..], out var uuid)
            )
            {
                result = new TypeId<TEntity>(uuid);
                return true;
            }
        }
        else if (_prefix.Length == 0 && s.Length == TypeIdConstants.IdLength)
        {
            if (UuidBase32.TryDecode(s, out var uuid))
            {
                result = new TypeId<TEntity>(uuid);
                return true;
            }
        }

        result = default;
        return false;
    }

    /// <inheritdoc/>
    public static TypeId<TEntity> Parse(string s, IFormatProvider? provider = null) =>
        TryParse(s.AsSpan(), provider, out var result)
            ? result
            : throw new FormatException($"Invalid TypeId<{typeof(TEntity).Name}> format: '{s}'");

    /// <inheritdoc cref="TryParse(ReadOnlySpan{char}, IFormatProvider?, out TypeId{TEntity})"/>
    public static bool TryParse([NotNullWhen(true)] string? s, out TypeId<TEntity> result) =>
        TryParse(s.AsSpan(), null, out result);

    /// <inheritdoc/>
    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out TypeId<TEntity> result
    ) => TryParse(s.AsSpan(), provider, out result);

    /// <inheritdoc/>
    public static TypeId<TEntity> Parse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider = null
    ) =>
        TryParse(utf8Text, provider, out var result)
            ? result
            : throw new FormatException(
                $"Invalid TypeId<{typeof(TEntity).Name}> format: '{Encoding.UTF8.GetString(utf8Text)}'"
            );

    /// <inheritdoc cref="TryParse(ReadOnlySpan{byte}, IFormatProvider?, out TypeId{TEntity})"/>
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out TypeId<TEntity> result) =>
        TryParse(utf8Text, null, out result);

    /// <inheritdoc/>
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out TypeId<TEntity> result
    )
    {
        if (!TypeId.TryParse(utf8Text, provider, out var typeId))
        {
            result = default;
            return false;
        }

        if (typeId.Prefix != _prefix)
        {
            result = default;
            return false;
        }

        // Prefix is already validated, use the ctor that does not validate prefix again
        result = new TypeId<TEntity>(typeId.Uuid);
        return true;
    }
}
