using RazorLight;
using Withly.Application.Emails.Templates;
using Withly.Persistence.Entities.Interfaces;

namespace Withly.Infrastructure.Email;

public class RazorTemplateRenderer : IEmailTemplateRenderer
{
    private readonly RazorLightEngine _engine = new RazorLightEngineBuilder()
        .UseEmbeddedResourcesProject(typeof(WelcomeEmail))
        .UseMemoryCachingProvider()
        .Build();

    public async Task<string> RenderAsync<T>(T model) where T : IEmailTemplate
    {
        return await _engine.CompileRenderAsync(model.TemplateName, model);
    }
}