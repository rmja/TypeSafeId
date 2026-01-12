# TypeSafeId

A modern, type-safe implementation of [TypeIDs](https://github.com/jetify-com/typeid) for .NET. TypeIds are globally unique, K-sortable identifiers that combine a human-readable prefix with a UUIDv7 encoded in Crockford Base32.

## Features

- ✨ **Type-safe** - Generic `TypeId<TEntity>` provides compile-time safety
- 🔤 **Human-readable** - Prefixes make identifiers self-describing (e.g., `user_01h455vb4pex5vsknk084sn02q`)
- ⏱️ **K-sortable** - UUIDv7 based, preserves insertion order
- 🚀 **AOT Compatible** - Ready for Native AOT compilation
- 🔗 **ASP.NET Core Integration** - Route constraints and model binding
- 💾 **Entity Framework Core Support** - Value converters for database storage

## Installation

```bash
# Core library
dotnet add package TypeId

# ASP.NET Core extensions
dotnet add package TypeId.AspNetCore

# Entity Framework Core extensions
dotnet add package TypeId.EntityFrameworkCore
```

## Quick Start

### Basic Usage

```csharp
using TypeId;

// Create a TypeId with a prefix
var userId = TypeId.Create("user");
Console.WriteLine(userId); // user_01h455vb4pex5vsknk084sn02q

// Parse from string
var parsed = TypeId.Parse("user_01h455vb4pex5vsknk084sn02q");
```

### Strongly-Typed TypeIds

```csharp
// Define your entity types
record User(TypeId<User> Id, string Name);

// By default, the prefix is derived from the type name (lowercase, "_" separated)
var user = new User(
    Id: TypeId<User>.Create(),
    Name: "John Doe"
);
Console.WriteLine(user.Id); // user_01h455vb4pex5vsknk084sn02q

// Customize the prefix with the TypeId attribute
[TypeId("prd")]
record Product(TypeId<Product> Id, string Name);

var product = new Product(
    Id: TypeId<Product>.Create(),
    Name: "Widget"
);
Console.WriteLine(product.Id); // prd_01h455vb4pex5vsknk084sn02q
```

## ASP.NET Core Integration

Add TypeId routing support in your application:

```csharp
using TypeId;

var builder = WebApplication.CreateBuilder(args);

// Register TypeId route constraints
builder.Services.AddTypeIdRouting();

var app = builder.Build();

// Use TypeId in route parameters
app.MapGet("/users/{userId:typeid}", (TypeId<User> userId) =>
{
    return Results.Ok(new { UserId = userId });
});

app.MapGet("/products/{productId:typeid}", (TypeId<Product> productId) =>
{
    return Results.Ok(new { ProductId = productId });
});

app.Run();

record User(TypeId<User> Id, string Name);

[TypeId("prd")]
record Product(TypeId<Product> Id, string Name);
```

## Entity Framework Core Integration

Use TypeId value converters for database storage:

```csharp
using Microsoft.EntityFrameworkCore;
using TypeId.EntityFrameworkCore.Storage.ValueConversion;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Store as string
        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .HasConversion(new TypeIdToStringConverter<User>());

        // Or store as bytes for better performance
        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .HasConversion(new TypeIdToBytesConverter<User>());
    }
}

record User
{
    public TypeId<User> Id { get; init; }
    public string Name { get; set; } = string.Empty;
}
```

## JSON Serialization

TypeIds serialize naturally with System.Text.Json:

```csharp
using System.Text.Json;
using TypeId;

var user = new User(
    Id: TypeId<User>.Create(),
    Name: "Jane Doe"
);

var json = JsonSerializer.Serialize(user);
// {"Id":"user_01h455vb4pex5vsknk084sn02q","Name":"Jane Doe"}

var deserialized = JsonSerializer.Deserialize<User>(json);
```

## Format Specification

TypeIds follow the [official specification](https://github.com/jetify-com/typeid):

- **Format**: `[prefix_]<base32-encoded-uuid>`
- **Prefix**: Optional, lowercase letters (a-z) and underscores, max 63 characters
- **Separator**: Single underscore `_` (when prefix is present)
- **UUID**: UUIDv7 encoded in Crockford Base32 (26 characters)
- **Total Length**: 26 characters (no prefix) or prefix length + 1 + 26

### Examples

```
user_01h455vb4pex5vsknk084sn02q     # With prefix "user"
order_01h455vb4pex5vsknk084sn02r    # With prefix "order"
01h455vb4pex5vsknk084sn02s          # Without prefix
```

## Related Projects

This implementation is inspired by and builds upon:

- [firenero/TypeId](https://github.com/firenero/TypeId) - Another .NET TypeId implementation
- [UUIDNext](https://github.com/mareek/UUIDNext) - High-performance UUID generation library for .NET, used for UUIDv7 generation

## License

MIT License - See [LICENSE](LICENSE) file for details

## Repository

[https://github.com/rmja/TypeId](https://github.com/rmja/TypeId)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
