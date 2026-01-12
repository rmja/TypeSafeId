namespace TypeSafeId.Tests;

public class TypeIdTests
{
    // Test entities with different attribute configurations
    [TypeId("user")]
    record UserWithExplicitPrefix(TypeId<UserWithExplicitPrefix> Id);

    [TypeId("")]
    record UserWithEmptyPrefix(TypeId<UserWithEmptyPrefix> Id);

    [TypeId]
    record UserWithDefaultPrefix(TypeId<UserWithDefaultPrefix> Id);

    record UserWithoutAttribute(TypeId<UserWithoutAttribute> Id);

    [TypeId("custom_prefix")]
    record EntityWithUnderscorePrefix(TypeId<EntityWithUnderscorePrefix> Id);

    [TypeId("a")]
    record EntityWithSingleCharPrefix(TypeId<EntityWithSingleCharPrefix> Id);

    [Fact]
    public void Prefix_WithExplicitAttribute_UsesSpecifiedPrefix()
    {
        // Arrange & Act
        var prefix = TypeId<UserWithExplicitPrefix>.Prefix;

        // Assert
        Assert.Equal("user", prefix);
    }

    [Fact]
    public void Prefix_WithEmptyAttributePrefix()
    {
        // Arrange & Act
        var prefix = TypeId<UserWithEmptyPrefix>.Prefix;

        // Assert
        Assert.Equal("", prefix);
    }

    [Fact]
    public void Prefix_WithDefaultAttribute_UsesTypeNameLowercased()
    {
        // Arrange & Act
        var prefix = TypeId<UserWithDefaultPrefix>.Prefix;

        // Assert
        Assert.Equal("user_with_default_prefix", prefix);
    }

    [Fact]
    public void Prefix_WithoutAttribute_UsesTypeNameLowercased()
    {
        // Arrange & Act
        var prefix = TypeId<UserWithoutAttribute>.Prefix;

        // Assert
        Assert.Equal("user_without_attribute", prefix);
    }

    [Fact]
    public void Prefix_WithUnderscoreInAttribute_UsesSpecifiedPrefix()
    {
        // Arrange & Act
        var prefix = TypeId<EntityWithUnderscorePrefix>.Prefix;

        // Assert
        Assert.Equal("custom_prefix", prefix);
    }

    [Fact]
    public void Prefix_WithSingleCharacter_UsesSpecifiedPrefix()
    {
        // Arrange & Act
        var prefix = TypeId<EntityWithSingleCharPrefix>.Prefix;

        // Assert
        Assert.Equal("a", prefix);
    }

    [Fact]
    public void Create_WithExplicitPrefix_CreatesTypeIdWithCorrectPrefix()
    {
        // Arrange & Act
        var typeId = TypeId<UserWithExplicitPrefix>.Create();
        var typeIdString = typeId.ToString();

        // Assert
        Assert.StartsWith("user_", typeIdString);
        Assert.Equal(31, typeIdString.Length); // "user_" (5) + 26 chars
    }

    [Fact]
    public void Create_WithDefaultPrefix_CreatesTypeIdWithCorrectPrefix()
    {
        // Arrange & Act
        var typeId = TypeId<UserWithDefaultPrefix>.Create();
        var typeIdString = typeId.ToString();

        // Assert
        Assert.StartsWith("user_with_default_prefix_", typeIdString);
    }

    [Fact]
    public void Parse_WithMatchingPrefix_ParsesSuccessfully()
    {
        // Arrange
        var original = TypeId<UserWithExplicitPrefix>.Create();
        var str = original.ToString();

        // Act
        var parsed = TypeId<UserWithExplicitPrefix>.Parse(str);

        // Assert
        Assert.Equal(original, parsed);
        Assert.Equal(original.Uuid, parsed.Uuid);
    }

    [Fact]
    public void Parse_WithMismatchedPrefix_ThrowsFormatException()
    {
        // Arrange
        var typeIdString = "wrongprefix_01h455vb4pex5vsknk084sn02q";

        // Act & Assert
        Assert.Throws<FormatException>(() => TypeId<UserWithExplicitPrefix>.Parse(typeIdString));
    }

    [Fact]
    public void TryParse_WithMatchingPrefix_ReturnsTrue()
    {
        // Arrange
        var original = TypeId<UserWithExplicitPrefix>.Create();
        var str = original.ToString();

        // Act
        var success = TypeId<UserWithExplicitPrefix>.TryParse(str, null, out var parsed);

        // Assert
        Assert.True(success);
        Assert.Equal(original, parsed);
    }

    [Fact]
    public void TryParse_WithMismatchedPrefix_ReturnsFalse()
    {
        // Arrange
        var typeIdString = "wrongprefix_01h455vb4pex5vsknk084sn02q";

        // Act
        var success = TypeId<UserWithExplicitPrefix>.TryParse(typeIdString, null, out var parsed);

        // Assert
        Assert.False(success);
        Assert.Equal(default, parsed);
    }

    [Fact]
    public void Constructor_FromGuid_CreatesTypeIdWithCorrectPrefix()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var typeId = new TypeId<UserWithExplicitPrefix>(guid);

        // Assert
        Assert.Equal(guid, typeId.Uuid);
        Assert.Equal("user", TypeId<UserWithExplicitPrefix>.Prefix);
        Assert.StartsWith("user_", typeId.ToString());
    }

    [Fact]
    public void Constructor_FromUntypedTypeId_WithMatchingPrefix_Succeeds()
    {
        // Arrange
        var untyped = TypeId.Create("user");

        // Act
        var typed = new TypeId<UserWithExplicitPrefix>(untyped);

        // Assert
        Assert.Equal(untyped.Uuid, typed.Uuid);
        Assert.Equal(untyped.ToString(), typed.ToString());
    }

    [Fact]
    public void Constructor_FromUntypedTypeId_WithMismatchedPrefix_ThrowsArgumentException()
    {
        // Arrange
        var untyped = TypeId.Create("wrongprefix");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new TypeId<UserWithExplicitPrefix>(untyped)
        );
        Assert.Contains("TypeId prefix must be 'user'", exception.Message);
    }

    [Fact]
    public void ImplicitConversion_ToUntypedTypeId_PreservesValue()
    {
        // Arrange
        var typed = TypeId<UserWithExplicitPrefix>.Create();

        // Act
        TypeId untyped = typed;

        // Assert
        Assert.Equal(typed.Uuid, untyped.Uuid);
        Assert.Equal(typed.ToString(), untyped.ToString());
    }

    [Fact]
    public void ExplicitConversion_FromUntypedTypeId_WithMatchingPrefix_Succeeds()
    {
        // Arrange
        var untyped = TypeId.Create("user");

        // Act
        var typed = (TypeId<UserWithExplicitPrefix>)untyped;

        // Assert
        Assert.Equal(untyped.Uuid, typed.Uuid);
        Assert.Equal(untyped.ToString(), typed.ToString());
    }

    [Fact]
    public void ExplicitConversion_FromUntypedTypeId_WithMismatchedPrefix_ThrowsArgumentException()
    {
        // Arrange
        var untyped = TypeId.Create("wrongprefix");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => (TypeId<UserWithExplicitPrefix>)untyped);
    }

    [Fact]
    public void MultipleInstances_UseSameCachedPrefix()
    {
        // Arrange & Act
        var prefix1 = TypeId<UserWithExplicitPrefix>.Prefix;
        var prefix2 = TypeId<UserWithExplicitPrefix>.Prefix;

        // Assert
        Assert.Same(prefix1, prefix2);
    }
}
