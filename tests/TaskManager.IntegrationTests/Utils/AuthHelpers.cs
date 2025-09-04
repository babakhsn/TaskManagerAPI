using System.Net.Http.Json;

namespace TaskManager.IntegrationTests.Utils;

public static class AuthHelpers
{
    public static async Task<string> RegisterAndLoginAsync(HttpClient client, string email, string password)
    {
        var reg = await client.PostAsJsonAsync("/api/auth/register", new { email, password });
        if (!reg.IsSuccessStatusCode)
        {
            // if already exists, try login
            var login = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
            login.EnsureSuccessStatusCode();
            var lr = await login.Content.ReadFromJsonAsync<AuthResponse>();
            return lr!.AccessToken;
        }

        var rr = await reg.Content.ReadFromJsonAsync<AuthResponse>();
        return rr!.AccessToken;
    }

    public static void SetBearer(this HttpClient client, string token)
        => client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

    private sealed record AuthResponse(string AccessToken);
}
