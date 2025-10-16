using DotNetEnv;
using PorquinhoApi.Endpoints;
using PorquinhoApi.Services;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.RegisterDatabaseService(builder.Configuration);
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(options =>
    {
        options.Title = "Porquinho API";
    });
}

app.MapUserEndpoints();
app.MapFunctionalitiesEndpoints();
app.MapSubscriptionTierEndpoints();
app.MapSubscriptionStatusEndpoints();
app.MapSubscriptionEndpoints();

app.Run();