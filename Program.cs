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

Env.Load();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/application-.log",
        rollingInterval: RollingInterval.Day
    )
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

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

app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(options =>
    {
        options.Title = "Porquinho API";
    });
}

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

app.Run();

public partial class Program { }
