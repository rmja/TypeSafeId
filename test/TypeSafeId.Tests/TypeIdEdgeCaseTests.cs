namespace TypeSafeId.Tests;

public class TypeIdEdgeCaseTests
{
    [TypeId("abc")]
    record ShortPrefix(TypeId<ShortPrefix> Id);

    [TypeId("a_very_long_prefix_which_is_actually_the_maximum_allowed_length")]
    record LongPrefix(TypeId<LongPrefix> Id);

    [TypeId("prefix_with_multiple_underscores_in_it")]
    record MultipleUnderscores(TypeId<MultipleUnderscores> Id);

    [Fact]
    public void Prefix_WithThreeCharacters_Works()
    {
        // Arrange & Act
        var prefix = TypeId<ShortPrefix>.Prefix;
        var typeId = TypeId<ShortPrefix>.Create();

        // Assert
        Assert.Equal("abc", prefix);
        Assert.StartsWith("abc_", typeId.ToString());
    }

    [Fact]
    public void Prefix_WithMaximumAllowedLength_Works()
    {
        // Arrange & Act
        var prefix = TypeId<LongPrefix>.Prefix;
        var typeId = TypeId<LongPrefix>.Create();

        // Assert
        Assert.Equal(63, prefix.Length);
        Assert.StartsWith(prefix + "_", typeId.ToString());
    }

    [Fact]
    public void Prefix_WithMultipleUnderscores_Works()
    {
        // Arrange & Act
        var prefix = TypeId<MultipleUnderscores>.Prefix;
        var typeId = TypeId<MultipleUnderscores>.Create();

        // Assert
        Assert.Equal("prefix_with_multiple_underscores_in_it", prefix);
        Assert.StartsWith("prefix_with_multiple_underscores_in_it_", typeId.ToString());
    }

    [Fact]
    public void Parse_WithMaxLengthTypeId_ParsesSuccessfully()
    {
        // Arrange
        var original = TypeId<LongPrefix>.Create();
        var str = original.ToString();

        // Act
        var parsed = TypeId<LongPrefix>.Parse(str);

        // Assert
        Assert.Equal(original, parsed);
        Assert.Equal(90, str.Length); // 63 + 1 + 26
    }

    [Fact]
    public void Equals_WithDefaultStruct_ReturnsFalse()
    {
        // Arrange
        var typeId = TypeId<ShortPrefix>.Create();
        var defaultValue = default(TypeId<ShortPrefix>);

        // Act & Assert
        Assert.NotEqual(typeId, defaultValue);
        Assert.False(typeId.Equals(defaultValue));
    }

    [Fact]
    public void CompareTo_WithNull_ReturnsPositive()
    {
        // Arrange
        var typeId = TypeId<ShortPrefix>.Create();

        // Act
        var result = typeId.CompareTo(null);

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public void CompareTo_WithWrongType_ThrowsArgumentException()
    {
        // Arrange
        var typeId = TypeId<ShortPrefix>.Create();
        object wrongType = "not a TypeId";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => typeId.CompareTo(wrongType));
    }

    [Fact]
    public void Parse_WithEmptyString_ThrowsFormatException()
    {
        // Arrange & Act & Assert
        Assert.Throws<FormatException>(() => TypeId<ShortPrefix>.Parse(""));
    }

    [Fact]
    public void TryParse_WithNullString_ReturnsFalse()
    {
        // Arrange & Act
        var success = TypeId<ShortPrefix>.TryParse((string?)null, null, out var result);

        // Assert
        Assert.False(success);
        Assert.Equal(default, result);
    }

    [Fact]
    public void Parse_WithWhitespace_ThrowsFormatException()
    {
        // Arrange & Act & Assert
        Assert.Throws<FormatException>(() => TypeId<ShortPrefix>.Parse("   "));
    }
}
