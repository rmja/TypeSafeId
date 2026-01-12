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
    )
    .WithName("GetWeatherForecast");

app.Run();

record User(TypeId<User> Id, string Name) : ITypeIdEntity
{
    public static string TypeIdPrefix => "user";
};
