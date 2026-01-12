namespace TypeId.Tests;

public class TypeIdInvalidPrefixTests
{
    [Fact]
    public void Prefix_WithUppercaseCharacters_ThrowsTypeInitializationException()
    {
        // Arrange & Act & Assert
        Assert.Throws<TypeInitializationException>(() =>
        {
            _ = TypeId<EntityWithUppercasePrefix>.Prefix;
        });
    }

    [Fact]
    public void Prefix_WithLeadingUnderscore_ThrowsTypeInitializationException()
    {
        // Arrange & Act & Assert
        Assert.Throws<TypeInitializationException>(() =>
        {
            _ = TypeId<EntityWithLeadingUnderscore>.Prefix;
        });
    }

    [Fact]
    public void Prefix_WithTrailingUnderscore_ThrowsTypeInitializationException()
    {
        // Arrange & Act & Assert
        Assert.Throws<TypeInitializationException>(() =>
        {
            _ = TypeId<EntityWithTrailingUnderscore>.Prefix;
        });
    }

    [Fact]
    public void Prefix_WithInvalidCharacters_ThrowsTypeInitializationException()
    {
        // Arrange & Act & Assert
        Assert.Throws<TypeInitializationException>(() =>
        {
            _ = TypeId<EntityWithInvalidChars>.Prefix;
        });
    }

    [Fact]
    public void Prefix_WithTooLongPrefix_ThrowsTypeInitializationException()
    {
        // Arrange & Act & Assert
        Assert.Throws<TypeInitializationException>(() =>
        {
            _ = TypeId<EntityWithTooLongPrefix>.Prefix;
        });
    }

    // Test entities with invalid prefixes
    [TypeId("User")]
    record EntityWithUppercasePrefix(TypeId<EntityWithUppercasePrefix> Id);

    [TypeId("_user")]
    record EntityWithLeadingUnderscore(TypeId<EntityWithLeadingUnderscore> Id);

    [TypeId("user_")]
    record EntityWithTrailingUnderscore(TypeId<EntityWithTrailingUnderscore> Id);

    [TypeId("user-name")]
    record EntityWithInvalidChars(TypeId<EntityWithInvalidChars> Id);

    [TypeId(
        "this_is_a_very_long_prefix_that_exceeds_the_maximum_allowed_length_of_sixty_three_characters"
    )]
    record EntityWithTooLongPrefix(TypeId<EntityWithTooLongPrefix> Id);
}
