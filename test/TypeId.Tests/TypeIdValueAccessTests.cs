namespace TypeId.Tests;

public class TypeIdValueAccessTests
{
    [TypeId("user")]
    record User(TypeId<User> Id);

    [Fact]
    public void Uuid_ReturnsUnderlyingGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var typeId = new TypeId<User>(guid);

        // Act
        var result = typeId.Uuid;

        // Assert
        Assert.Equal(guid, result);
    }

    [Fact]
    public void Value_ReturnsUntypedTypeId()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var typed = new TypeId<User>(guid);

        // Act
        var untyped = typed.Value;

        // Assert
        Assert.Equal(typed.Uuid, untyped.Uuid);
        Assert.Equal(typed.ToString(), untyped.ToString());
        Assert.Equal("user", untyped.Prefix);
    }

    [Fact]
    public void Prefix_IsAccessibleFromStaticProperty()
    {
        // Arrange & Act
        var prefix = TypeId<User>.Prefix;

        // Assert
        Assert.Equal("user", prefix);
    }

    [Fact]
    public void Create_WithTimestamp_CreatesTypeIdWithSpecificTimestamp()
    {
        // Arrange
        var timestamp = new DateTimeOffset(2026, 1, 12, 10, 30, 0, TimeSpan.Zero);

        // Act
        var typeId1 = TypeId<User>.Create(timestamp);
        var typeId2 = TypeId<User>.Create(timestamp);

        // Assert
        // Both should have UUIDs generated from the same timestamp
        // Note: They won't be equal due to random component, but should be close
        Assert.NotEqual(typeId1, typeId2);
        Assert.StartsWith("user_", typeId1.ToString());
        Assert.StartsWith("user_", typeId2.ToString());
    }

    [Fact]
    public void Create_WithoutTimestamp_UsesCurrentTime()
    {
        // Arrange
        var before = TypeId<User>.Create(DateTimeOffset.UtcNow);

        // Act
        var typeId = TypeId<User>.Create();

        // Assert
        var after = TypeId<User>.Create(DateTimeOffset.UtcNow);
        Assert.StartsWith("user_", typeId.ToString());

        // The UUID should be created within the time range
        Assert.NotEqual(Guid.Empty, typeId.Uuid);
        Assert.True(before < typeId);
        Assert.True(typeId < after);
    }
}
