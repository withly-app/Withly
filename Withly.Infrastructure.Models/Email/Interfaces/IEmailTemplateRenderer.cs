namespace Withly.Infrastructure.Models.Email.Interfaces;

public interface IEmailTemplateRenderer
{
    Task<string> RenderAsync<T>(T model) where T : IEmailTemplate;
}