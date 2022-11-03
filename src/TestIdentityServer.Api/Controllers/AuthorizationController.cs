using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace TestIdentityServer.Api.Controllers;

public class AuthorizationController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOpenIddictScopeManager _openIddictScopeManager;

    public AuthorizationController(IHttpContextAccessor httpContextAccessor, IOpenIddictScopeManager openIddictScopeManager)
    {
        this._httpContextAccessor = httpContextAccessor;
        this._openIddictScopeManager = openIddictScopeManager;
    }

    [HttpPost("/connect/token")]
    public async Task<IActionResult> Exchange()
    {
        var request = this._httpContextAccessor.HttpContext!.GetOpenIddictServerRequest()
                      ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (!request.IsClientCredentialsGrantType())
            throw new InvalidOperationException("The specified grant type is not supported.");

        var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());

        identity.AddClaim("some-claim", "some-value", OpenIddictConstants.Destinations.AccessToken);

        var claimsPrincipal = new ClaimsPrincipal(identity);

        var scopes = request.GetScopes();
        var resources = await this._openIddictScopeManager.ListResourcesAsync(scopes).ToListAsync();
        claimsPrincipal.SetScopes(scopes);
        claimsPrincipal.SetResources(resources);

        return this.SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
