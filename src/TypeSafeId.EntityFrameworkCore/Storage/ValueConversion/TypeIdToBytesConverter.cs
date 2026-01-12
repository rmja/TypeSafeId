using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TypeSafeId.EntityFrameworkCore.Storage.ValueConversion;

public class TypeIdToBytesConverter<TEntity>()
    : ValueConverter<TypeId<TEntity>, byte[]>(
        convertToProviderExpression: x => x.Uuid.ToByteArray(bigEndian: true),
        convertFromProviderExpression: x => new TypeId<TEntity>(new Guid(x, bigEndian: true)),
        new RelationalConverterMappingHints(size: 16, fixedLength: true)
    );
