using DotNetEnv;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PorquinhoApi.Endpoints;
using PorquinhoApi.Services;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Context;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PorquinhoApi.Models;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var mongoUrl = builder.Configuration["MongoDb:ConnectionString"];

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/application-.log",
        rollingInterval: RollingInterval.Day
    )
    .WriteTo.MongoDB(
        databaseUrl: mongoUrl!,
        collectionName: "api_logs"
    )
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddSingleton<ApiLogService>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.RegisterDatabaseService(builder.Configuration);
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHateoasLinkService, HateoasLinkService>();

builder.Services.AddSingleton<ImportedTransactionService>();

var connectionString = builder.Configuration.GetConnectionString("OracleConnection");

builder.Services.AddHealthChecks()
    .AddOracle(
        connectionString: connectionString!,
        name: "oracle-database",
        failureStatus: HealthStatus.Degraded,
        tags: ["db", "oracle", "sql"],
        timeout: TimeSpan.FromSeconds(10)
    );

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(
        serviceName: builder.Environment.ApplicationName))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddConsoleExporter();
    });

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"]!;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)
            ),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<JwtTokenService>();

var app = builder.Build();

Log.Information("Application started successfully.");

if (app.Environment.IsDevelopment())
{
    Log.Warning("The application is running in Development environment.");
}

app.Use(async (context, next) =>
{
    var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                        ?? Guid.NewGuid().ToString();

    context.Items["CorrelationId"] = correlationId;
    context.Response.Headers["X-Correlation-ID"] = correlationId;

    using (LogContext.PushProperty("CorrelationId", correlationId))
    {
        await next();
    }
});

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (ctx, http) =>
    {
        ctx.Set("UserAgent", http.Request.Headers.UserAgent.ToString());
        ctx.Set("CorrelationId", http.Items["CorrelationId"]);
        ctx.Set("RequestHost", http.Request.Host.Value);
        ctx.Set("RequestScheme", http.Request.Scheme);
    };
});

app.MapApiLogEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(options =>
    {
        options.Title = "Porquinho API";
    });
}

app.MapAuthEndpoints();

app.MapHealthChecks("/health");

app.MapHealthChecks("/health/database", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db"),
});

app.MapUserEndpoints();
app.MapFunctionalitiesEndpoints();
app.MapSubscriptionTierEndpoints();
app.MapSubscriptionStatusEndpoints();
app.MapSubscriptionEndpoints();
app.MapImportedTransactionEndpoints();

app.Run();

public partial class Program { }
