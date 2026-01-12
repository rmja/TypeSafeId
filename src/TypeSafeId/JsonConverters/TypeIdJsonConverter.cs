using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TypeSafeId.JsonConverters;

/// <summary>
/// JSON converter for TypeId values.
/// </summary>
public class TypeIdJsonConverter : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert == typeof(TypeId)
        || (
            RuntimeFeature.IsDynamicCodeSupported
            && typeToConvert.IsGenericType
            && typeToConvert.GetGenericTypeDefinition() == typeof(TypeId<>)
        );

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (typeToConvert == typeof(TypeId))
        {
            return new InternalTypeIdJsonConverter();
        }

        if (!RuntimeFeature.IsDynamicCodeSupported)
        {
            return null;
        }

        var genericArgument = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(TypeIdJsonConverter<>).MakeGenericType(genericArgument);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

class InternalTypeIdJsonConverter : JsonConverter<TypeId>
{
    /// <inheritdoc/>
    public override TypeId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        return ReadTypeId(ref reader);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TypeId value, JsonSerializerOptions options)
    {
        Span<char> buffer = stackalloc char[TypeIdConstants.MaxLength];
        var length = value.CopyTo(buffer);
        writer.WriteStringValue(buffer[..length]);
    }

    /// <inheritdoc/>
    public override TypeId ReadAsPropertyName(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        return ReadTypeId(ref reader);
    }

    /// <inheritdoc/>
    public override void WriteAsPropertyName(
        Utf8JsonWriter writer,
        [DisallowNull] TypeId value,
        JsonSerializerOptions options
    )
    {
        Span<char> buffer = stackalloc char[TypeIdConstants.MaxLength];
        var length = value.CopyTo(buffer);
        writer.WritePropertyName(buffer[..length]);
    }

    private static TypeId ReadTypeId(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException(
                $"Unexpected token parsing TypeId. Expected String, got {reader.TokenType}."
            );
        }

        var valueSpan = reader.ValueSpan;
        if (!TypeId.TryParse(valueSpan, null, out var typeId))
        {
            throw new JsonException($"Invalid TypeId format: {reader.GetString()}");
        }

        return typeId;
    }
}
