namespace Withly.Application.DevAuth;

public class DevAuthOptions
{
    public bool Enabled { get; set; }
    public DevUserOptions User { get; set; } = new();
    public string[] Roles { get; set; } = Array.Empty<string>();
    public int TokenLifetimeMinutes { get; set; } = 60;

    public sealed class DevUserOptions
    {
        public string Email { get; set; } = "";
        public string Name { get; set; } = "Developer";
        public string Password { get; set; } = "Test123!";
    }
}