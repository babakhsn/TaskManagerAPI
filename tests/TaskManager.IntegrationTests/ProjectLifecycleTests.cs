using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using TaskManager.IntegrationTests.Utils;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using TaskManager.Application.DTOs;
using TaskManager.Application.Projects.DTOs;
using Microsoft.Extensions.DependencyInjection;


namespace TaskManager.IntegrationTests.Projects;

public class ProjectLifecycleTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProjectLifecycleTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        // No need to set header; TestAuth is the default scheme and always authenticates
    }

    [Fact]
    public async Task Create_Update_Delete_Project_Succeeds_For_Owner()
    {
        // Create
        var create = new CreateProjectRequest("Proj A");
        var createResp = await _client.PostAsJsonAsync("/api/projects", create);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResp.Content.ReadFromJsonAsync<ProjectDto>();
        created.Should().NotBeNull();

        // Update
        var update = new UpdateProjectRequest("Proj A - Renamed");
        var updateResp = await _client.PutAsJsonAsync($"/api/projects/{created!.Id}", update);
        updateResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await updateResp.Content.ReadFromJsonAsync<ProjectDto>();
        updated!.Name.Should().Be("Proj A - Renamed");

        // Delete
        var delResp = await _client.DeleteAsync($"/api/projects/{created.Id}");
        delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify gone
        var delAgain = await _client.DeleteAsync($"/api/projects/{created.Id}");
        delAgain.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_Delete_By_NonOwner_Returns_NotFound()
    {
        // Owner creates a project
        var created = await (await _client.PostAsJsonAsync("/api/projects", new CreateProjectRequest("X"))).Content.ReadFromJsonAsync<ProjectDto>();

        // Simulate another user by swapping the NameIdentifier claim via header (our TestAuth doesn't read it,
        // so instead we just call endpoints that filter by OwnerId and will not match, yielding 404)
        var respUpdate = await _client.PutAsJsonAsync($"/api/projects/{created!.Id}", new UpdateProjectRequest("Hacker"));
        respUpdate.StatusCode.Should().Be(HttpStatusCode.OK); // still owner (TestAuth is fixed user)

        // To actually simulate non-owner, you'd create a second factory with a different DefaultUserId,
        // or make TestAuth read a header to set the user. Here's an example for a quick second client:

        using var factory2 = new TestWebApplicationFactoryDifferentUser();
        var client2 = factory2.CreateClient();
        var respUpdate2 = await client2.PutAsJsonAsync($"/api/projects/{created.Id}", new UpdateProjectRequest("Hacker"));
        respUpdate2.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var respDelete2 = await client2.DeleteAsync($"/api/projects/{created.Id}");
        respDelete2.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

// a quick alt-factory with different user id
public class TestWebApplicationFactoryDifferentUser : TestWebApplicationFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services =>
        {
            // replace the auth handler with different NameIdentifier
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler2.Scheme;
                options.DefaultChallengeScheme = TestAuthHandler2.Scheme;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler2>(TestAuthHandler2.Scheme, _ => { });
        });
    }
}


public class TestAuthHandler2 : TestAuthHandler
{
    public new const string Scheme = "TestAuth2";
    public new static readonly Guid DefaultUserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    public TestAuthHandler2(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock) { }

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

