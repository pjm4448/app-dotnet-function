using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PjmCore.Functions.HealthChecks;

var builder = FunctionsApplication.CreateBuilder(args);

// --- ASP.NET Core Integration ---
// Enables HttpRequest/IActionResult pattern for HTTP-triggered functions.
builder.ConfigureFunctionsWebApplication();

// --- Application Insights ---
builder.Services.AddApplicationInsightsTelemetryWorkerService();

// --- BlobServiceClient Registration ---
// Conditional: Managed Identity in Azure, Azurite connection string locally.
var storageAccountName = builder.Configuration["AzureWebJobsStorage:accountName"];

if (!string.IsNullOrEmpty(storageAccountName))
{
    // Azure: identity-based access via System-Assigned Managed Identity
    builder.Services.AddSingleton(new BlobServiceClient(
        new Uri($"https://{storageAccountName}.blob.core.windows.net"),
        new DefaultAzureCredential()));
}
else
{
    // Local: Azurite emulator via connection string
    var connectionString = builder.Configuration["AzureWebJobsStorage"]
        ?? "UseDevelopmentStorage=true";
    builder.Services.AddSingleton(new BlobServiceClient(connectionString));
}

// --- Health Checks ---
builder.Services
    .AddHealthChecks()
    .AddCheck<StorageHealthCheck>("storage");

builder.Build().Run();
