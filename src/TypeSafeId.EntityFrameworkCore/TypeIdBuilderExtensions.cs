using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TypeSafeId.EntityFrameworkCore.Storage.ValueConversion;

namespace TypeSafeId.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for configuring Entity Framework Core property builders to use TypeId value conversions.
/// </summary>
public static class TypeIdBuilderExtensions
{
    /// <summary>
    /// Configures the property to use a byte array conversion for the TypeId value when storing and retrieving data
    /// from the database.
    /// </summary>
    /// <remarks>This method enables Entity Framework Core to persist TypeId&lt;TEntity&gt; values as byte arrays in
    /// the underlying data store. Use this conversion when you want to store only the uuid part of the strongly typed identifier as binary data
    /// rather than as string.</remarks>
    /// <typeparam name="TEntity">The entity type that the TypeId is associated with.</typeparam>
    /// <param name="builder">The builder used to configure the property of type TypeId&lt;TEntity&gt;.</param>
    /// <returns>The same PropertyBuilder instance so that additional configuration calls can be chained.</returns>
    public static PropertyBuilder<TypeId<TEntity>> HasTypeIdToBytesConversion<TEntity>(
        this PropertyBuilder<TypeId<TEntity>> builder
    ) => builder.HasConversion(new TypeIdToBytesConverter<TEntity>());

    /// <inheritdoc cref="HasTypeIdToBytesConversion{TEntity}(PropertyBuilder{TypeId{TEntity}})"/>
    public static PropertyBuilder<TypeId<TEntity>?> HasTypeIdToBytesConversion<TEntity>(
        this PropertyBuilder<TypeId<TEntity>?> builder
    ) => builder.HasConversion(new TypeIdToBytesConverter<TEntity>());

    /// <summary>
    /// Configures the property to use a string conversion for the TypeId value when storing and retrieving data from
    /// the database.
    /// </summary>
    /// <remarks>This method enables Entity Framework Core to persist TypeId values as strings in the
    /// underlying database. Use this conversion when you want to store the entire strongly-typed identifier as string (including prefix) for
    /// compatibility or readability.</remarks>
    /// <typeparam name="TEntity">The entity type that the TypeId is associated with.</typeparam>
    /// <param name="builder">The builder used to configure the property of type TypeId for the entity.</param>
    /// <returns>The same PropertyBuilder instance so that additional configuration calls can be chained.</returns>
    public static PropertyBuilder<TypeId<TEntity>> HasTypeIdToStringConversion<TEntity>(
        this PropertyBuilder<TypeId<TEntity>> builder
    ) => builder.HasConversion(new TypeIdToStringConverter<TEntity>());

    /// <inheritdoc cref="HasTypeIdToStringConversion{TEntity}(PropertyBuilder{TypeId{TEntity}})"/>
    public static PropertyBuilder<TypeId<TEntity>?> HasTypeIdToStringConversion<TEntity>(
        this PropertyBuilder<TypeId<TEntity>?> builder
    ) => builder.HasConversion(new TypeIdToStringConverter<TEntity>());

    /// <summary>
    /// Configures the property to use a string-based conversion for the TypeId type when storing values in the
    /// database.
    /// </summary>
    /// <remarks>This method enables Entity Framework Core to persist TypeId values as strings in the
    /// underlying database. Use this conversion when you want to store TypeId values in a human-readable format or when
    /// the database schema requires string storage.</remarks>
    /// <param name="builder">The builder for the property of type TypeId to configure.</param>
    /// <returns>The same PropertyBuilder instance so that additional configuration calls can be chained.</returns>
    public static PropertyBuilder<TypeId> HasTypeIdToStringConversion(
        this PropertyBuilder<TypeId> builder
    ) => builder.HasConversion(new TypeIdToStringConverter());

    /// <inheritdoc cref="HasTypeIdToStringConversion(PropertyBuilder{TypeId})"/>
    public static PropertyBuilder<TypeId?> HasTypeIdToStringConversion(
        this PropertyBuilder<TypeId?> builder
    ) => builder.HasConversion(new TypeIdToStringConverter());
}
