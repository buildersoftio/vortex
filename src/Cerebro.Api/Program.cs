
using Cerebro.Api.DependencyInjection;
using Cerebro.Api.Middleware;
using Cerebro.Infrastructure.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

// adding Serilog
builder.Host.UseSerilog();


// ---------------------------------------------------------------------
// Configure Services
// ---------------------------------------------------------------------

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v4", new OpenApiInfo
    {
        Title = "Cerebro | Engine",
        Version = "v4",
        Description = "Buildersoft Cerebro is an open-source distributed streaming platform designed to deliver the best performance possible for high-performance data pipelines, streaming analytics, streaming between microservices and data integration.",
        License = new OpenApiLicense() { Name = "Licensed under the Apache License 2.0", Url = new Uri("https://bit.ly/3DqVQbx") }

    });
});

// Adding IO Services
builder.Services.AddConfigurations(builder.Configuration);
builder.Services.AddSystemStarterService();
builder.Services.AddIOServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v4/swagger.json", "Cerebro | Engine v4"));
}

app.UseMiddleware<RestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseSystemStarterService();

app.Run();
