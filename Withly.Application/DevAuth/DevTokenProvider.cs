namespace Withly.Application.DevAuth;

public sealed class DevTokenProvider : IDevTokenProvider
{
    public override string? Token { get; internal set; }
}