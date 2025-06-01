using Microsoft.EntityFrameworkCore;
using Serilog;
using Withly.API.Extensions;
using Withly.API.Middleware;
using Withly.Application;
using Withly.Infrastructure;
using Withly.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilog(builder.Configuration);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAppHealthChecks();
builder.Services.AddControllers();


builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
        
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();
app.UseAppHealthChecks();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}