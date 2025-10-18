using Serilog;
using Withly.API.Extensions;
using Withly.Application.DevAuth;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilog(builder.Configuration);
builder.Configuration.AddUserSecrets<Program>(optional: true);

builder.Services.AddWithlyApi(builder.Configuration);

#if DEBUG
if (builder.Environment.IsDevelopment())
{
    builder.Services.Configure<DevAuthOptions>(builder.Configuration.GetSection("DevAuth"));
    builder.Services.AddSingleton<DevTokenProvider>();
    builder.Services.AddSingleton<IDevTokenProvider>(sp => sp.GetRequiredService<DevTokenProvider>());
    builder.Services.AddHostedService<DevUserSeeder>();
}
#endif

var app = builder.Build();

app.UseWithlyApi();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}