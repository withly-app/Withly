namespace Withly.Application.DevAuth;

public class IDevTokenProvider
{
    public virtual string? Token { get; internal set; }
}