using Cerebro.Server.DependencyInjection;
using Cerebro.Server.Handlers;
using Cerebro.Server.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;

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

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // it works but for now, we are not enabling JSON Text for Controllers
    });

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

    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[] {}
        }
    });
});

// Adding IO Services
builder.Services.AddConfigurations(builder.Configuration);

builder.Services.AddGRPCClusterServer();
builder.Services.AddGRPCBrokerServer();

builder.Services.AddSystemStarterService();
builder.Services.AddIOServices();
builder.Services.AddOrchestators();

builder.Services.AddAuthentication("Cerebro_Authentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Cerebro_Authentication", null);

// Adding Factories
builder.Services.AddFactories();

// Adding Server State related components
builder.Services.AddServerStateStore();
builder.Services.AddServerRepositories();
builder.Services.AddServerStateServices();
builder.Services.AddBackgroundServerStateServices();
builder.Services.AddBackgroundTimerServerStateServices();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v4/swagger.json", "Cerebro | Engine v4"));
}

app.UseMiddleware<RestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSystemStarterService();

app.Run();
