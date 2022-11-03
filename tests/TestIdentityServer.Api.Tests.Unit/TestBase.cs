using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace TestIdentityServer.Api.Tests.Unit;

public class TestBase
{
    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly IServiceProvider _provider;

    protected TestBase()
    {
        this._services.AddServices();

        this.UnregisterActualHttpContextAccessor();
        this.SetupHttpContextAccessorMock();

        this._provider = this._services.BuildServiceProvider();
    }

    protected Mock<HttpContext> HttpContextMock { get; private set; } = null!;

    protected T FindService<T>()
        where T : notnull
    {
        return this._provider.GetRequiredService<T>();
    }

    private void UnregisterActualHttpContextAccessor()
    {
        var httpContextAccessor = this._services.Single(s => s.ServiceType == typeof(IHttpContextAccessor));
        this._services.Remove(httpContextAccessor);
    }

    private void SetupHttpContextAccessorMock()
    {
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        this.HttpContextMock = new Mock<HttpContext>();

        httpContextAccessorMock.SetupGet(hca => hca.HttpContext).Returns(this.HttpContextMock.Object);

        this._services.AddSingleton(httpContextAccessorMock.Object);
    }
}
