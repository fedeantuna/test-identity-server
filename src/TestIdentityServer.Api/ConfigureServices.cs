using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TestIdentityServer.Api.Controllers;
using TestIdentityServer.Api.Workers;

namespace TestIdentityServer.Api;

public static class ConfigureServices
{
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services
            .AddControllers()
            .AddControllersAsServices();
        services.AddTransient<AuthorizationController>();

        services.ConfigurePersistence();

        services.ConfigureOpenIddict();

        services.AddHostedService<TestDataWorker>();

        return services;
    }

    private static void ConfigurePersistence(this IServiceCollection services)
    {
        services.AddDbContext<DbContext>(options =>
        {
            options.UseInMemoryDatabase(nameof(DbContext));

            options.UseOpenIddict();
        });
    }

    private static void ConfigureOpenIddict(this IServiceCollection services)
    {
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<DbContext>();
            })
            .AddServer(options =>
            {
                options.AllowClientCredentialsFlow();
                options.SetTokenEndpointUris("/connect/token");

                options
                    .AddEphemeralEncryptionKey()
                    .AddEphemeralSigningKey()
                    .DisableAccessTokenEncryption();

                options.RegisterScopes("test-scope");

                options
                    .UseAspNetCore()
                    .EnableTokenEndpointPassthrough();
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });
    }
}
