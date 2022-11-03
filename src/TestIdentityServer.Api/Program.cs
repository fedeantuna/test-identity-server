using TestIdentityServer.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServices();

var app = builder.Build();
app.SetupMiddleware();

app.Run();
