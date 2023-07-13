
using Cerebro.Api.DependencyInjection;
using Cerebro.Infrastructure.DependencyInjection;
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
builder.Services.AddSwaggerGen();

// Adding IO Services
builder.Services.AddConfigurations(builder.Configuration);
builder.Services.AddSystemStarterService();
builder.Services.AddIOServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseSystemStarterService();

app.Run();
