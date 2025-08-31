namespace TaskManager.Domain.Common;

public static class Guard
{
    public static void AgainstNullOrEmpty(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be empty.", paramName);
    }

    public static void AgainstPast(DateTime? value, string paramName)
    {
        if (value.HasValue && value.Value < DateTime.UtcNow.Date)
            throw new ArgumentException($"{paramName} cannot be in the past.", paramName);
    }
}
