namespace TestIdentityServer.Api;

public static class Configure
{
    public static void SetupMiddleware(this WebApplication app)
    {
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}
