using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace EventPhotographer.Tests;

internal class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SCHEME = "TestScheme";
    public const string USER_ID_HEADER = "X-Test-UserId";
    public const string USER_EMAIL_HEADER = "X-Test-UserEmail";

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger, UrlEncoder encoder) 
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var userId = Request.Headers[USER_ID_HEADER].FirstOrDefault() ?? "1";
        var email = Request.Headers[USER_EMAIL_HEADER].FirstOrDefault() ?? "test@test.com";

        var claims = new[] { 
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.NameIdentifier, userId),
        };

        var identity = new ClaimsIdentity(claims, SCHEME);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SCHEME);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
