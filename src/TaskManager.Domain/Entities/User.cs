using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public class User : EntityBase
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty; // placeholder for Day 4
    public string Role { get; private set; } = "User";

    // ctor
    public User(string email)
    {
        Domain.Common.Guard.AgainstNullOrEmpty(email, nameof(email));
        Email = email.Trim().ToLowerInvariant();
    }
}
