namespace TypeSafeId.Tests;

public class TypeIdInteroperabilityTests
{
    [TypeId("user")]
    record User(TypeId<User> Id);

    [TypeId("user")]
    record AnotherUserType(TypeId<AnotherUserType> Id);

    [Fact]
    public void DifferentTypesWithSamePrefix_CreateDifferentTypeIds()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var user = new TypeId<User>(guid);
        var anotherUser = new TypeId<AnotherUserType>(guid);

        // Assert
        Assert.Equal(guid, user.Uuid);
        Assert.Equal(guid, anotherUser.Uuid);
        Assert.Equal(user.ToString(), anotherUser.ToString());
    }

    [Fact]
    public void TypeId_CanBeConvertedBetweenCompatibleTypes()
    {
        // Arrange
        var user = TypeId<User>.Create();
        TypeId untyped = user;

        // Act
        var anotherUser = new TypeId<AnotherUserType>(untyped);

        // Assert
        Assert.Equal(user.Uuid, anotherUser.Uuid);
        Assert.Equal(user.ToString(), anotherUser.ToString());
    }

    [Fact]
    public void UntypedTypeId_CreatedWithSamePrefix_CanBeConvertedToTyped()
    {
        // Arrange
        var untyped = TypeId.Create("user");

        // Act
        var typed = new TypeId<User>(untyped);

        // Assert
        Assert.Equal(untyped.Uuid, typed.Uuid);
        Assert.Equal(untyped.Prefix, TypeId<User>.Prefix);
    }

    [Fact]
    public void TypeIdAttribute_DoesNotAffectUntypedTypeId()
    {
        // Arrange
        var untyped1 = TypeId.Create("user");
        var untyped2 = TypeId.Create("user");

        // Act & Assert
        Assert.NotEqual(untyped1, untyped2); // Different UUIDs
        Assert.Equal("user", untyped1.Prefix);
        Assert.Equal("user", untyped2.Prefix);
    }
}
