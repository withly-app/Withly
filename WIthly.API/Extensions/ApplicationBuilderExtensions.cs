using Serilog;
using Withly.API.Middleware;
using Withly.Application.DevAuth;

namespace Withly.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseWithlyApi(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        #if DEBUG
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(o => { o.InjectJavascript("/swagger-dev-auth.js"); });

            app.MapGet("/swagger-dev-auth.js", () =>
                Results.Text("""
                             (async () => {
                               // get your token first
                               let token = "";
                               try {
                                 const r = await fetch("/dev/token");
                                 token = (await r.text()).trim();
                               } catch (e) {
                                 console.error("Token fetch failed", e);
                               }
                               if (!token) return;

                               // wait for Swagger UI to be ready
                               function isReady() {
                                 const isReady = !!(window.ui && typeof window.ui.preauthorizeApiKey === "function")
                                 console.log("Swagger UI ready?", isReady);
                                 return isReady;
                               }

                               const started = Date.now();
                               const timeoutMs = 10000; // safety net
                               const id = setInterval(() => {
                                 if (isReady()) {
                                   clearInterval(id);
                                   try {
                                     window.ui.preauthorizeApiKey("Bearer", token);
                                     console.log("Preauthorized Swagger UI");
                                   } catch (e) {
                                     console.error("Preauth failed after ready", e);
                                   }
                                 } else if (Date.now() - started > timeoutMs) {
                                   clearInterval(id);
                                   console.warn("Swagger UI didn't become ready in time");
                                 }
                               }, 500);
                             })();
                             """, "application/javascript")
            ).AllowAnonymous();
            
            app.Use(async (ctx, next) =>
            {
              try { await next(); }
              catch (TaskCanceledException) when (ctx.Request.Path.StartsWithSegments("/swagger"))
              {
                // Client closed request; swallow to avoid 500 spam
                // Optionally: ctx.Response.StatusCode = 499; // non-standard but descriptive
              }
            });
            
            app.MapGet("/dev/token",
                (IDevTokenProvider p) => string.IsNullOrWhiteSpace(p.Token)
                  ? Results.NotFound("Token not generated")
                  : Results.Text(p.Token, "text/plain"))
              .AllowAnonymous();
        }
        #endif
        
        app.UseHttpsRedirection();

        app.UseMiddleware<ExceptionHandlerMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.UseAppHealthChecks();

        return app;
    }
}