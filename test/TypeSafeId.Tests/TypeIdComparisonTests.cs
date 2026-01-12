namespace TypeSafeId.Tests;

public class TypeIdComparisonTests
{
    [TypeId("user")]
    record User(TypeId<User> Id);

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var typeId1 = new TypeId<User>(guid);
        var typeId2 = new TypeId<User>(guid);

        // Act & Assert
        Assert.Equal(typeId1, typeId2);
        Assert.True(typeId1.Equals(typeId2));
        Assert.True(typeId1 == typeId2);
        Assert.False(typeId1 != typeId2);
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        // Arrange
        var typeId1 = TypeId<User>.Create();
        var typeId2 = TypeId<User>.Create();

        // Act & Assert
        Assert.NotEqual(typeId1, typeId2);
        Assert.False(typeId1.Equals(typeId2));
        Assert.False(typeId1 == typeId2);
        Assert.True(typeId1 != typeId2);
    }

    [Fact]
    public void CompareTo_SmallerTimestamp_ReturnsNegative()
    {
        // Arrange
        var timestamp1 = DateTimeOffset.UtcNow;
        var timestamp2 = timestamp1.AddSeconds(1);
        var typeId1 = TypeId<User>.Create(timestamp1);
        var typeId2 = TypeId<User>.Create(timestamp2);

        // Act
        var result = typeId1.CompareTo(typeId2);

        // Assert
        Assert.True(result < 0);
        Assert.True(typeId1 < typeId2);
        Assert.True(typeId1 <= typeId2);
        Assert.False(typeId1 > typeId2);
        Assert.False(typeId1 >= typeId2);
    }

    [Fact]
    public void CompareTo_LargerTimestamp_ReturnsPositive()
    {
        // Arrange
        var timestamp1 = DateTimeOffset.UtcNow.AddSeconds(1);
        var timestamp2 = DateTimeOffset.UtcNow;
        var typeId1 = TypeId<User>.Create(timestamp1);
        var typeId2 = TypeId<User>.Create(timestamp2);

        // Act
        var result = typeId1.CompareTo(typeId2);

        // Assert
        Assert.True(result > 0);
        Assert.True(typeId1 > typeId2);
        Assert.True(typeId1 >= typeId2);
        Assert.False(typeId1 < typeId2);
        Assert.False(typeId1 <= typeId2);
    }

    [Fact]
    public void CompareTo_SameValue_ReturnsZero()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var typeId1 = new TypeId<User>(guid);
        var typeId2 = new TypeId<User>(guid);

        // Act
        var result = typeId1.CompareTo(typeId2);

        // Assert
        Assert.Equal(0, result);
        Assert.True(typeId1 <= typeId2);
        Assert.True(typeId1 >= typeId2);
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHashCode()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var typeId1 = new TypeId<User>(guid);
        var typeId2 = new TypeId<User>(guid);

        // Act
        var hash1 = typeId1.GetHashCode();
        var hash2 = typeId2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void CanBeUsedInHashSet()
    {
        // Arrange
        var typeId1 = TypeId<User>.Create();
        var typeId2 = TypeId<User>.Create();
        var typeId3 = new TypeId<User>(typeId1.Uuid);

        var set = new HashSet<TypeId<User>> { typeId1, typeId2 };

        // Act & Assert
        Assert.Equal(2, set.Count);
        Assert.Contains(typeId1, set);
        Assert.Contains(typeId2, set);
        Assert.Contains(typeId3, set); // Same as typeId1
    }

    [Fact]
    public void CanBeUsedAsDictionaryKey()
    {
        // Arrange
        var typeId1 = TypeId<User>.Create();
        var typeId2 = TypeId<User>.Create();
        var dictionary = new Dictionary<TypeId<User>, string>
        {
            { typeId1, "User 1" },
            { typeId2, "User 2" },
        };

        // Act & Assert
        Assert.Equal(2, dictionary.Count);
        Assert.Equal("User 1", dictionary[typeId1]);
        Assert.Equal("User 2", dictionary[typeId2]);
    }
}
