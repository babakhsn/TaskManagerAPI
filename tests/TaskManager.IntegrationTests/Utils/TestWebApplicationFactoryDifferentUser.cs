using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using TaskManager.IntegrationTests.Utils;

public class TestWebApplicationFactoryDifferentUser : TestWebApplicationFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler2.Scheme;
                options.DefaultChallengeScheme = TestAuthHandler2.Scheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler2>(
                TestAuthHandler2.Scheme, _ => { });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(TestAuthHandler2.Scheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });
        });
    }
}

[method: Obsolete]
public class TestAuthHandler2(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock) : TaskManager.IntegrationTests.Utils.TestAuthHandler(options, logger, encoder, clock)
{
    public new const string Scheme = "TestAuth2";
    public new static readonly Guid DefaultUserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, DefaultUserId.ToString()),
            new Claim(ClaimTypes.Email, "user2@example.com")
        };
        var identity = new ClaimsIdentity(claims, Scheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
