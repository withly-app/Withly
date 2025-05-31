using Withly.Persistence;

namespace Withly.Application.Auth.Interfaces;
public interface IAuthTokenGenerator
{
    string Generate(ApplicationUser user);
}