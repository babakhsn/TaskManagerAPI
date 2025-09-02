﻿using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TaskManager.IntegrationTests.Utils;

[method: Obsolete]
public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
    public new const string Scheme = "TestAuth";
    public static readonly Guid DefaultUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, DefaultUserId.ToString()),
            new Claim(ClaimTypes.Email, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, Scheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
