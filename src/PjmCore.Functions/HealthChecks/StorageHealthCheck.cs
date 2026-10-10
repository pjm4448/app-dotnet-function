using Azure.Storage.Blobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PjmCore.Functions.HealthChecks;

public class StorageHealthCheck : IHealthCheck
{
    public const string ContainerName = "deploymentpackage";

    private readonly BlobServiceClient _blobServiceClient;

    public StorageHealthCheck(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
            await containerClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            return HealthCheckResult.Healthy("Storage account is accessible.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Storage account is not accessible.",
                exception: ex);
        }
    }
}
