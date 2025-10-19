namespace Withly.Domain;

public static class SecretGenerator
{
    /// <summary>
    /// Generates a secure random secret string.
    /// </summary>
    /// <returns></returns>
    public static string GenerateSecret()
    {
        var bytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}