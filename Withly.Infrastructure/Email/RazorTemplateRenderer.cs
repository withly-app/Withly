using Microsoft.Extensions.Logging;
using RazorLight;
using Withly.Application.Common.Interfaces;
using Withly.Application.Emails.Templates;

namespace Withly.Infrastructure.Email;

public class RazorTemplateRenderer(ILogger<RazorTemplateRenderer> logger) : IEmailTemplateRenderer
{
    private readonly RazorLightEngine _engine = new RazorLightEngineBuilder()
        .UseEmbeddedResourcesProject(typeof(WelcomeEmail))
        .UseMemoryCachingProvider()
        .Build();

    public async Task<string?> RenderAsync<T>(T model) where T : IEmailTemplate
    {
        try
        {
            return await _engine.CompileRenderAsync(model.TemplateName, model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while rendering template");
        }
        return null;
    }
}