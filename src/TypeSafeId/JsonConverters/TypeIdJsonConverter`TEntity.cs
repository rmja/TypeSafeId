using System.Text.Json;
using System.Text.Json.Serialization;
using TypeSafeId;

namespace TypeSafeId.JsonConverters;

/// <summary>
/// JSON converter for strongly-typed TypeId.
/// </summary>
public class TypeIdJsonConverter<TEntity> : JsonConverter<TypeId<TEntity>>
{
    /// <inheritdoc/>
    public override TypeId<TEntity> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException(
                $"Unexpected token parsing TypeId<{typeof(TEntity).Name}>. Expected String, got {reader.TokenType}."
            );
        }

        var valueSpan = reader.ValueSpan;
        if (!TypeId<TEntity>.TryParse(valueSpan, null, out var typeId))
        {
            throw new JsonException(
                $"Invalid TypeId<{typeof(TEntity).Name}> format: {reader.GetString()}"
            );
        }

        return typeId;
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        TypeId<TEntity> value,
        JsonSerializerOptions options
    )
    {
        Span<char> buffer = stackalloc char[TypeIdConstants.MaxLength];
        var length = value.CopyTo(buffer);
        writer.WriteStringValue(buffer[..length]);
    }

    /// <inheritdoc/>
    public override TypeId<TEntity> ReadAsPropertyName(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException(
                $"Unexpected token parsing TypeId<{typeof(TEntity).Name}> property name. Expected PropertyName, got {reader.TokenType}."
            );
        }

        var valueSpan = reader.ValueSpan;
        if (!TypeId<TEntity>.TryParse(valueSpan, null, out var typeId))
        {
            throw new JsonException(
                $"Invalid TypeId<{typeof(TEntity).Name}> format: {reader.GetString()}"
            );
        }

        return typeId;
    }

    /// <inheritdoc/>
    public override void WriteAsPropertyName(
        Utf8JsonWriter writer,
        TypeId<TEntity> value,
        JsonSerializerOptions options
    )
    {
        Span<char> buffer = stackalloc char[TypeIdConstants.MaxLength];
        var length = value.CopyTo(buffer);
        writer.WritePropertyName(buffer[..length]);
    }
}
