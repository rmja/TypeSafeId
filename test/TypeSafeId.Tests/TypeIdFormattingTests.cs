namespace TypeSafeId.Tests;

public class TypeIdFormattingTests
{
    [TypeId("user")]
    record User(TypeId<User> Id);

    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var guid = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
        var typeId = new TypeId<User>(guid);

        // Act
        var result = typeId.ToString();

        // Assert
        Assert.StartsWith("user_", result);
        Assert.Equal(TypeId<User>.Prefix.Length + 1 + 26, result.Length);
    }

    [Fact]
    public void ToString_WithFormatProvider_ReturnsCorrectFormat()
    {
        // Arrange
        var typeId = TypeId<User>.Create();

        // Act
        var result = typeId.ToString(null, null);

        // Assert
        Assert.StartsWith("user_", result);
    }

    [Fact]
    public void TryFormat_WithSufficientBuffer_ReturnsTrue()
    {
        // Arrange
        var typeId = TypeId<User>.Create();
        Span<char> buffer = stackalloc char[100];

        // Act
        var success = typeId.TryFormat(buffer, out var charsWritten, default, null);

        // Assert
        Assert.True(success);
        Assert.Equal(typeId.Length, charsWritten);
        Assert.Equal(typeId.ToString(), new string(buffer[..charsWritten]));
    }

    [Fact]
    public void TryFormat_WithInsufficientBuffer_ReturnsFalse()
    {
        // Arrange
        var typeId = TypeId<User>.Create();
        Span<char> buffer = stackalloc char[10]; // Too small

        // Act
        var success = typeId.TryFormat(buffer, out var charsWritten, default, null);

        // Assert
        Assert.False(success);
        Assert.Equal(0, charsWritten);
    }

    [Fact]
    public void CopyTo_CopiesCorrectly()
    {
        // Arrange
        var typeId = TypeId<User>.Create();
        Span<char> buffer = stackalloc char[100];

        // Act
        var charsWritten = typeId.CopyTo(buffer);

        // Assert
        Assert.Equal(typeId.Length, charsWritten);
        Assert.Equal(typeId.ToString(), new string(buffer[..charsWritten]));
    }

    [Fact]
    public void Length_ReturnsCorrectValue()
    {
        // Arrange
        var typeId = TypeId<User>.Create();

        // Act
        var length = typeId.Length;

        // Assert
        Assert.Equal("user_".Length + 26, length);
    }
}
