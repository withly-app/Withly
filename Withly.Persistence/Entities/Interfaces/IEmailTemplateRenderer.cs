namespace Withly.Persistence.Entities.Interfaces;

public interface IEmailTemplateRenderer
{
    Task<string> RenderAsync<T>(T model) where T : IEmailTemplate;
}