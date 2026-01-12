using TypeId;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTypeIdRouting();

var app = builder.Build();

// Example: /users/user_01h455vb4pex5vsknk084sn02q
app.MapGet(
    "/users/{userId:typeid}",
    (TypeId<User> userId) =>
    {
        return userId;
    }
);

// Example: /products/prd_01h455vb4pex5vsknk084sn02q
app.MapGet(
    "/products/{productId:typeid}",
    (TypeId<Product> productId) =>
    {
        return productId;
    }
);
app.Run();

// Without attributes, TypeId<User> will have the prefix "user"
record User(TypeId<User> Id, string Name);

// With attributes, TypeId<Product> will have the prefix "prd"
[TypeId("prd")]
record Product(TypeId<Product> Id, string Name);
