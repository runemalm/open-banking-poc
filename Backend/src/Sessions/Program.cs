using DDD.Infrastructure.Repositories.EF.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Hangfire;
using Hangfire.PostgreSql;
using Demo.Application.Actions.CreateSession;
using Demo.Application.Actions.GetBanks;
using Demo.Application.Actions.GetIntegrations;
using Demo.Application.Actions.GetSession;
using Demo.Application.Actions.GetState;
using Demo.Application.Actions.ProvideInput;
using Demo.Application.Actions.SelectBank;
using Demo.Application.Actions.SelectIntegration;
using Demo.Application.Actions.StartSession;
using Demo.Domain.Model;
using Demo.Domain.Model.Bank;
using Demo.Domain.Model.Input;
using Demo.Domain.Model.Integration;
using Demo.Domain.Model.StateMachine.Factory;
using Demo.Domain.Services;
using Demo.Infrastructure.Integrations.Se.Klarna;
using Demo.Infrastructure.Integrations.Se.Seb;
using Demo.Infrastructure.Integrations.Se.Swedbank;
using Demo.Infrastructure.Repositories.EF;
using Demo.Infrastructure.Repositories.EF.Configurations;
using Demo.Infrastructure.Repositories.EF.Context;
using Demo.Infrastructure.Repositories.EF.Seeders;

var builder = WebApplication.CreateBuilder(args);

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
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
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
builder.Services.AddDbContext<SessionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Enable Hangfire Dashboard
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter() } // Optional security
    });
}

app.UseHttpsRedirection();

// Use the cors policy
app.UseCors("AllowAll");

// Add controllers to the pipeline
app.MapControllers();

// Run the app
app.Run();
