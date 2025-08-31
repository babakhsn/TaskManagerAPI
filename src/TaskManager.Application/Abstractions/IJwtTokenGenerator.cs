using System.Security.Claims;

namespace TaskManager.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string email, IEnumerable<Claim>? extraClaims = null);
}
