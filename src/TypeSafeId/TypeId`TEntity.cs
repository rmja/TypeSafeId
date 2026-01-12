using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeSafeId.JsonConverters;

namespace TypeSafeId;

/// <summary>
/// Represents a strongly-typed TypeId that is constrained to a specific entity type.
/// </summary>
/// <typeparam name="TEntity">The entity type that defines the TypeId prefix.</typeparam>
[JsonConverter(typeof(TypeIdJsonConverter))]
public readonly struct TypeId<TEntity>
    : IEquatable<TypeId<TEntity>>,
        ISpanParsable<TypeId<TEntity>>,
        IUtf8SpanParsable<TypeId<TEntity>>,
        ISpanFormattable,
        IComparable<TypeId<TEntity>>,
        IComparable
{
    private readonly TypeId _value;

    /// <summary>
    /// Gets the prefix string defined by the associated attribute.
    /// </summary>
    /// <remarks>The value is determined at runtime by evaluating the attribute applied to the containing type
    /// or member. This field is read-only.</remarks>
    public static readonly string Prefix = GetPrefixFromAttribute();

    /// <summary>
    /// Gets the underlying TypeId value.
    /// </summary>
    public TypeId Value => _value;

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
        _value = new TypeId(Prefix, uuid);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeId{TEntity}"/> struct from an untyped TypeId.
    /// </summary>
    /// <param name="value">The TypeId value.</param>
    /// <exception cref="ArgumentException">Thrown when the prefix doesn't match the entity type.</exception>
    public TypeId(TypeId value)
    {
        if (value.Prefix != Prefix)
        {
            throw new ArgumentException(
                $"TypeId prefix must be '{Prefix}', got '{value.Prefix}'",
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
        new(TypeId.Create(Prefix, timestamp));

    private static string GetPrefixFromAttribute()
    {
        var attribute = typeof(TEntity).GetCustomAttribute<TypeIdAttribute>();
        var prefix = attribute?.Prefix;

        // If no attribute or empty prefix, use the type name as default
        prefix ??= JsonNamingPolicy.SnakeCaseLower.ConvertName(typeof(TEntity).Name);

        var error = TypeId.Parser.ValidatePrefix(prefix);
        if (error != TypeId.Parser.PrefixError.None)
        {
            throw new InvalidOperationException(
                $"TypeId prefix for entity type '{typeof(TEntity).Name}' is invalid, error: {error}."
            );
        }

        return prefix;
    }

    /// <summary>
    /// Copies the string representation to a character span.
    /// </summary>
    public int CopyTo(Span<char> buffer) => _value.CopyTo(buffer);

    /// <inheritdoc/>
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
    public override int GetHashCode() => _value.GetHashCode();

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

    /// <inheritdoc/>
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out TypeId<TEntity> result
    )
    {
        if (!TypeId.TryParse(s, provider, out var typeId))
        {
            result = default;
            return false;
        }

        if (typeId.Prefix != Prefix)
        {
            result = default;
            return false;
        }

        result = new TypeId<TEntity>(typeId);
        return true;
    }

    /// <inheritdoc/>
    public static TypeId<TEntity> Parse(string s, IFormatProvider? provider) =>
        TryParse(s.AsSpan(), provider, out var result)
            ? result
            : throw new FormatException($"Invalid TypeId<{typeof(TEntity).Name}> format: '{s}'");

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

        if (typeId.Prefix != Prefix)
        {
            result = default;
            return false;
        }

        result = new TypeId<TEntity>(typeId);
        return true;
    }
}
