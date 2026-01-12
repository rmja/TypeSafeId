using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TypeSafeId.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Value converter for storing TypeId&lt;TEntity&gt; as a string in the database.
/// </summary>
/// <typeparam name="TEntity">The entity type that defines the TypeId prefix.</typeparam>
public class TypeIdToStringConverter<TEntity>()
    : ValueConverter<TypeId<TEntity>, string>(
        convertToProviderExpression: x => x.ToString(),
        convertFromProviderExpression: x => TypeId<TEntity>.Parse(x),
        new RelationalConverterMappingHints(
            size: TypeId.MaxLength,
            dbType: System.Data.DbType.AnsiString
        )
    );
