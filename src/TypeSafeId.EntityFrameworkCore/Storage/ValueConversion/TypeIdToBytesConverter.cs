using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TypeSafeId.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Value converter for storing TypeId&lt;TEntity&gt; as bytes in the database for better performance.
/// Stores the underlying UUID as a 16-byte array.
/// </summary>
/// <typeparam name="TEntity">The entity type that defines the TypeId prefix.</typeparam>
public class TypeIdToBytesConverter<TEntity>()
    : ValueConverter<TypeId<TEntity>, byte[]>(
        convertToProviderExpression: x => x.Uuid.ToByteArray(bigEndian: true),
        convertFromProviderExpression: x => new TypeId<TEntity>(new Guid(x, bigEndian: true)),
        new RelationalConverterMappingHints(size: 16, fixedLength: true)
    );
