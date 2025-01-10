using DDD.Infrastructure.Repositories.EF.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Hangfire;
using Hangfire.PostgreSql;
using Sessions.Application.Actions.CreateSession;
using Sessions.Application.Actions.GetBanks;
using Sessions.Application.Actions.GetIntegrations;
using Sessions.Application.Actions.GetSession;
using Sessions.Application.Actions.GetState;
using Sessions.Application.Actions.ProvideInput;
using Sessions.Application.Actions.SelectBank;
using Sessions.Application.Actions.SelectIntegration;
using Sessions.Application.Actions.StartSession;
using Sessions.Domain.Model;
using Sessions.Domain.Model.Bank;
using Sessions.Domain.Model.Input;
using Sessions.Domain.Model.Integration;
using Sessions.Domain.Model.StateMachine.Factory;
using Sessions.Domain.Services;
using Sessions.Infrastructure.Integrations.Se.Klarna;
using Sessions.Infrastructure.Integrations.Se.Seb;
using Sessions.Infrastructure.Integrations.Se.Swedbank;
using Sessions.Infrastructure.Repositories.EF;
using Sessions.Infrastructure.Repositories.EF.Configurations;
using Sessions.Infrastructure.Repositories.EF.Context;
using Sessions.Infrastructure.Repositories.EF.Seeders;
using Hangfire.MemoryStorage;

var builder = WebApplication.CreateBuilder(args);

// Add appsettings and environment variables
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Configure dependencies
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Open Banking",
        Version = "v1",
        Description = "This is a custom description for the API",
        Contact = new OpenApiContact
        {
            Name = "David Runemalm",
            Email = "david.runemalm@gmail.com"
        }
    });
});

// Hangfire configuration
builder.Services.AddHangfire(config =>
{
    if (builder.Environment.IsDevelopment())
    {
        config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        config.UseMemoryStorage();
    }
    
    GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
});
builder.Services.AddHangfireServer();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configure serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Centralized configuration for JSON serialization
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Add repositories
if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("Using PostgreSQL Database...");
    builder.Services.AddDbContext<SessionDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    Console.WriteLine("Using In-Memory Database...");
    builder.Services.AddDbContext<SessionDbContext>(options =>
        options.UseInMemoryDatabase("InMemoryDb"));
}

EfDbContextConfiguration.AdditionalAssemblies.Add(typeof(SessionConfiguration).Assembly);

builder.Services.AddScoped<ISessionDbContext>(provider => provider.GetRequiredService<SessionDbContext>());
builder.Services.AddScoped<IBankRepository, EfBankRepository>();
builder.Services.AddScoped<IIntegrationRepository, EfIntegrationRepository>();
builder.Services.AddScoped<ISessionRepository, EfSessionRepository>();
builder.Services.AddScoped<IInputRepository, EfInputRepository>();

// Add actions
builder.Services.AddTransient<GetBanksAction>();
builder.Services.AddTransient<GetIntegrationsAction>();
builder.Services.AddTransient<CreateSessionAction>();
builder.Services.AddTransient<SelectBankAction>();
builder.Services.AddTransient<SelectIntegrationAction>();
builder.Services.AddTransient<StartSessionAction>();
builder.Services.AddTransient<ProvideInputAction>();
builder.Services.AddTransient<GetStateAction>();
builder.Services.AddTransient<GetSessionAction>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

// State machine
builder.Services.AddTransient<IStateMachineFactory, StateMachineFactory>();

// Bank adapters
builder.Services.AddScoped<SeSeb01>();
builder.Services.AddScoped<SeSwedbank01>();
builder.Services.AddScoped<SeKlarna01>();

// Domain Services
builder.Services.AddScoped<ISessionDomainService, SessionDomainService>();

// Build the web application
var app = builder.Build();

// Seed data
await DatabaseSeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

// Enable Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter() } // Optional security
});

app.UseHttpsRedirection();

// Use the cors policy
app.UseCors("AllowAll");

// Add controllers to the pipeline
app.MapControllers();

// Run the app
app.Run();
