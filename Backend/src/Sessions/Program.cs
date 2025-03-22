using Microsoft.OpenApi.Models;
using OpenDDD.API.Extensions;
using OpenDDD.Infrastructure.Persistence.EfCore.Seeders;
using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.MemoryStorage;
using Sessions;
using Sessions.Domain.Model.StateMachine.Factory;
using Sessions.Infrastructure.Integrations.Se.Klarna;
using Sessions.Infrastructure.Integrations.Se.Seb;
using Sessions.Infrastructure.Integrations.Se.Swedbank;
using Sessions.Infrastructure.Persistence.EfCore.Context;
using Sessions.Infrastructure.Persistence.EfCore.Seeders;

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
        Description = "The open-banking-poc API",
        Contact = new OpenApiContact
        {
            Name = "David Runemalm",
            Email = "david.runemalm@gmail.com"
        }
    });
});

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

builder.Services.AddOpenDDD<SessionDbContext>(builder.Configuration, 
    options =>  
    {
        
    },
    services =>
    {
        services.AddScoped<SeSeb01>();
        services.AddScoped<SeSwedbank01>();
        services.AddScoped<SeKlarna01>();
    }
);

// State machine
builder.Services.AddTransient<IStateMachineFactory, StateMachineFactory>();

// Add Hangfire Services
var hangfireOptions = new HangfireOptions();
builder.Configuration.GetSection("Hangfire").Bind(hangfireOptions);

builder.Services.AddHangfire(config =>
{
    switch (hangfireOptions.StorageType.ToLower())
    {
        case "postgres":
            config.UsePostgreSqlStorage(hangfireOptions.ConnectionString);
            break;
        default:
            config.UseMemoryStorage();
            break;
    }

    GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
});

builder.Services.AddHangfireServer();

// Build the application
var app = builder.Build();

// Use OpenDDD Middleware
app.UseOpenDDD();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

// Enable Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAllowAllAuthFilter() }
    // Authorization = new[] { new HangfireBasicAuthFilter("admin", "password") }
    // Authorization = new[] { new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter() }
});

app.UseHttpsRedirection();

// Use the cors policy
app.UseCors("AllowAll");

// Add controllers actions to endpoints
app.MapControllers();

// Manually map a route to a method invocation
app.Map("/health", appBuilder => appBuilder.Run(async context =>
{
    await context.Response.WriteAsync("Healthy");
}));

app.Run();
