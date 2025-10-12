using Serilog;
using Withly.API.Extensions;
using Withly.Application.DevAuth; // <-- new namespace with our extensions

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilog(builder.Configuration);
builder.Configuration.AddUserSecrets<Program>(optional: true);

builder.Services.AddWithlyApi(builder.Configuration);

if (builder.Environment.IsDevelopment())
{
    builder.Services.Configure<DevAuthOptions>(builder.Configuration.GetSection("DevAuth"));
    builder.Services.AddSingleton<DevTokenProvider>();
    builder.Services.AddSingleton<IDevTokenProvider>(sp => sp.GetRequiredService<DevTokenProvider>());
    builder.Services.AddHostedService<DevUserSeeder>();
}

var app = builder.Build();

app.UseWithlyApi();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/dev/token",
            (IDevTokenProvider p) => string.IsNullOrWhiteSpace(p.Token)
                ? Results.NotFound("Token not generated")
                : Results.Text(p.Token, "text/plain"))
        .AllowAnonymous();
}

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}