using System.Text.Json;

namespace TypeId.Tests;

public class TypeIdSerializationTests
{
    [TypeId("user")]
    record User(TypeId<User> Id, string Name);

    [TypeId]
    record Product(TypeId<Product> Id, string Name, decimal Price);

    [Fact]
    public void Serialize_EntityWithExplicitPrefix_SerializesCorrectly()
    {
        // Arrange
        var user = new User(TypeId<User>.Create(), "John Doe");

        // Act
        var json = JsonSerializer.Serialize(user);
        var deserialized = JsonSerializer.Deserialize<User>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(user.Id, deserialized.Id);
        Assert.Equal(user.Name, deserialized.Name);
        Assert.Contains("\"Id\":\"user_", json);
    }

    [Fact]
    public void Serialize_EntityWithDefaultPrefix_SerializesCorrectly()
    {
        // Arrange
        var product = new Product(TypeId<Product>.Create(), "Widget", 19.99m);

        // Act
        var json = JsonSerializer.Serialize(product);
        var deserialized = JsonSerializer.Deserialize<Product>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(product.Id, deserialized.Id);
        Assert.Equal(product.Name, deserialized.Name);
        Assert.Equal(product.Price, deserialized.Price);
        Assert.Contains("\"Id\":\"product_", json);
    }

    [Fact]
    public void Deserialize_WithMatchingPrefix_DeserializesSuccessfully()
    {
        // Arrange
        var json = """{"Id":"user_01h455vb4pex5vsknk084sn02q","Name":"Jane Doe"}""";

        // Act
        var user = JsonSerializer.Deserialize<User>(json);

        // Assert
        Assert.NotNull(user);
        Assert.Equal("Jane Doe", user.Name);
        Assert.StartsWith("user_", user.Id.ToString());
    }

    [Fact]
    public void Deserialize_WithMismatchedPrefix_ThrowsJsonException()
    {
        // Arrange
        var json = """{"Id":"wrongprefix_01h455vb4pex5vsknk084sn02q","Name":"Jane Doe"}""";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<User>(json));
    }

    [Fact]
    public void SerializeDeserialize_RoundTrip_PreservesValue()
    {
        // Arrange
        var original = new User(TypeId<User>.Create(), "Test User");

        // Act
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<User>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(original.Id, deserialized.Id);
        Assert.Equal(original.Id.Uuid, deserialized.Id.Uuid);
        Assert.Equal(original.Name, deserialized.Name);
    }

    [Fact]
    public void Serialize_TypeIdAsPropertyName_SerializesCorrectly()
    {
        // Arrange
        var userId = TypeId<User>.Create();
        var dictionary = new Dictionary<TypeId<User>, string> { { userId, "John Doe" } };

        // Act
        var json = JsonSerializer.Serialize(dictionary);
        var deserialized = JsonSerializer.Deserialize<Dictionary<TypeId<User>, string>>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Single(deserialized);
        Assert.True(deserialized.ContainsKey(userId));
        Assert.Equal("John Doe", deserialized[userId]);
    }
}
