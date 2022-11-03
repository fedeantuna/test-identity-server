using FluentAssertions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using TestIdentityServer.Api.Controllers;

namespace TestIdentityServer.Api.Tests.Unit.Controllers;

public class AuthorizationControllerTests : TestBase
{
    private readonly AuthorizationController _sut;

    public AuthorizationControllerTests()
    {
        this._sut = this.FindService<AuthorizationController>();
    }

    [Fact]
    public async Task Exchange_ThrowsInvalidOperationException_WhenTransactionIsNull()
    {
        // Arrange
        var features = new FeatureCollection();
        features.Set(new OpenIddictServerAspNetCoreFeature
        {
            Transaction = null
        });

        this.HttpContextMock.SetupGet(hc => hc.Features).Returns(features);

        // Act
        var act = () => this._sut.Exchange();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("The OpenID Connect request cannot be retrieved.");
    }

    [Fact]
    public async Task  Exchange_ThrowsInvalidOperationException_WhenRequestWithinTransactionIsNull()
    {
        // Arrange
        var features = new FeatureCollection();
        features.Set(new OpenIddictServerAspNetCoreFeature
        {
            Transaction = new OpenIddictServerTransaction
            {
                Request = null
            }
        });

        this.HttpContextMock.SetupGet(hc => hc.Features).Returns(features);

        // Act
        var act = () => this._sut.Exchange();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("The OpenID Connect request cannot be retrieved.");
    }

    [Theory]
    [InlineData(OpenIddictConstants.GrantTypes.Implicit)]
    [InlineData(OpenIddictConstants.GrantTypes.Password)]
    [InlineData(OpenIddictConstants.GrantTypes.AuthorizationCode)]
    [InlineData(OpenIddictConstants.GrantTypes.DeviceCode)]
    [InlineData(OpenIddictConstants.GrantTypes.RefreshToken)]
    public async Task  Exchange_ThrowsInvalidOperationException_WhenRequestGrandTypeIsNotClientCredentials(string grantType)
    {
        // Arrange
        var features = new FeatureCollection();
        features.Set(new OpenIddictServerAspNetCoreFeature
        {
            Transaction = new OpenIddictServerTransaction
            {
                Request = new OpenIddictRequest
                {
                    GrantType = grantType
                }
            }
        });

        this.HttpContextMock.SetupGet(hc => hc.Features).Returns(features);

        // Act
        var act = () => this._sut.Exchange();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("The specified grant type is not supported.");
    }

    [Fact]
    public async Task  Exchange_ReturnsAccessToken_WhenRequestGrandTypeIsClientCredentials()
    {
        // Arrange
        const string testClientId = "test-client-id";

        var features = new FeatureCollection();
        features.Set(new OpenIddictServerAspNetCoreFeature
        {
            Transaction = new OpenIddictServerTransaction
            {
                Request = new OpenIddictRequest
                {
                    ClientId = testClientId,
                    GrantType = OpenIddictConstants.GrantTypes.ClientCredentials
                }
            }
        });

        this.HttpContextMock.SetupGet(hc => hc.Features).Returns(features);

        // Act
        var result = (SignInResult)await this._sut.Exchange();

        // Assert
        result.AuthenticationScheme.Should().Be(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        result.Principal.Claims.Should().Contain(c =>
            c.Type == OpenIddictConstants.Claims.Subject
            && c.Value == testClientId);
        result.Principal.Claims.Should().Contain(c =>
            c.Type == "some-claim"
            && c.Value == "some-value");
    }
}
