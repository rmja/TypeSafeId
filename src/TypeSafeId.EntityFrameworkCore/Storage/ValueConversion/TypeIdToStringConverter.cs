using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TypeSafeId.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Value converter for storing TypeId as a string in the database.
/// </summary>
public class TypeIdToStringConverter()
    : ValueConverter<TypeId, string>(
        convertToProviderExpression: x => x.ToString(),
        convertFromProviderExpression: x => TypeId.Parse(x),
        new RelationalConverterMappingHints(size: TypeId.MaxLength)
    );
