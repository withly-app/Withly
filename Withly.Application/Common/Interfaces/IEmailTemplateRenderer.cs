namespace Withly.Application.Common.Interfaces;

public interface IEmailTemplateRenderer
{
    Task<string?> RenderAsync<T>(T model) where T : IEmailTemplate;
}