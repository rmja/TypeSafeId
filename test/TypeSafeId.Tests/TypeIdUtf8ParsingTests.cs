using System.Text;

namespace TypeSafeId.Tests;

public class TypeIdUtf8ParsingTests
{
    [TypeId("user")]
    record User(TypeId<User> Id);

    [TypeId]
    record Product(TypeId<Product> Id);

    [Fact]
    public void Parse_Utf8WithMatchingPrefix_ParsesSuccessfully()
    {
        // Arrange
        var original = TypeId<User>.Create();
        var utf8Bytes = Encoding.UTF8.GetBytes(original.ToString());

        // Act
        var parsed = TypeId<User>.Parse(utf8Bytes);

        // Assert
        Assert.Equal(original, parsed);
        Assert.Equal(original.Uuid, parsed.Uuid);
    }

    [Fact]
    public void Parse_Utf8WithMismatchedPrefix_ThrowsFormatException()
    {
        // Arrange
        var typeIdString = "wrongprefix_01h455vb4pex5vsknk084sn02q";
        var utf8Bytes = Encoding.UTF8.GetBytes(typeIdString);

        // Act & Assert
        Assert.Throws<FormatException>(() => TypeId<User>.Parse(utf8Bytes));
    }

    [Fact]
    public void TryParse_Utf8WithMatchingPrefix_ReturnsTrue()
    {
        // Arrange
        var original = TypeId<User>.Create();
        var utf8Bytes = Encoding.UTF8.GetBytes(original.ToString());

        // Act
        var success = TypeId<User>.TryParse(utf8Bytes, null, out var parsed);

        // Assert
        Assert.True(success);
        Assert.Equal(original, parsed);
    }

    [Fact]
    public void TryParse_Utf8WithMismatchedPrefix_ReturnsFalse()
    {
        // Arrange
        var typeIdString = "wrongprefix_01h455vb4pex5vsknk084sn02q";
        var utf8Bytes = Encoding.UTF8.GetBytes(typeIdString);

        // Act
        var success = TypeId<User>.TryParse(utf8Bytes, null, out var parsed);

        // Assert
        Assert.False(success);
        Assert.Equal(default, parsed);
    }

    [Fact]
    public void TryParse_Utf8WithInvalidUtf8_ReturnsFalse()
    {
        // Arrange
        byte[] invalidUtf8 = [0xFF, 0xFE, 0xFD];

        // Act
        var success = TypeId<User>.TryParse(invalidUtf8, null, out var parsed);

        // Assert
        Assert.False(success);
        Assert.Equal(default, parsed);
    }

    [Fact]
    public void Parse_Utf8WithDefaultPrefix_ParsesSuccessfully()
    {
        // Arrange
        var original = TypeId<Product>.Create();
        var utf8Bytes = Encoding.UTF8.GetBytes(original.ToString());

        // Act
        var parsed = TypeId<Product>.Parse(utf8Bytes);

        // Assert
        Assert.Equal(original, parsed);
        Assert.StartsWith("product_", parsed.ToString());
    }
}
