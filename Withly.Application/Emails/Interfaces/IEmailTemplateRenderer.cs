namespace Withly.Application.Emails.Interfaces;

public interface IEmailTemplateRenderer
{
    Task<string?> RenderAsync<T>(T model) where T : IEmailTemplate;
}