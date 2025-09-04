using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManager.IntegrationTests.Utils;

namespace TaskManager.IntegrationTests.Auth;

public class AuthFlowTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    public AuthFlowTests(TestWebApplicationFactory f) => _client = f.CreateClient();

    [Fact]
    public async Task Register_Login_Me_Succeeds()
    {
        var email = $"ci_{Guid.NewGuid():N}@task.local";
        var pwd = "Passw0rd!";

        var token = await AuthHelpers.RegisterAndLoginAsync(_client, email, pwd);
        token.Should().NotBeNullOrEmpty();

        _client.SetBearer(token);
        var me = await _client.GetAsync("/api/me");
        me.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await me.Content.ReadAsStringAsync();
        body.Should().Contain("authorized");
    }
}
