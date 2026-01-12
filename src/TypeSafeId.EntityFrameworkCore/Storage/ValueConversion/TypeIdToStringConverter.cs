using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TypeSafeId.EntityFrameworkCore.Storage.ValueConversion;

public class TypeIdToStringConverter()
    : ValueConverter<TypeId, string>(
        convertToProviderExpression: x => x.ToString(),
        convertFromProviderExpression: x => TypeId.Parse(x),
        new RelationalConverterMappingHints(size: TypeId.MaxLength)
    );
