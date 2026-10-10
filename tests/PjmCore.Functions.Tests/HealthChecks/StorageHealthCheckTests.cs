using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using PjmCore.Functions.HealthChecks;

namespace PjmCore.Functions.Tests.HealthChecks;

public class StorageHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_ReturnsHealthy_WhenStorageIsAccessible()
    {
        // Arrange
        var mockClient = new Mock<BlobServiceClient>();
        mockClient
            .Setup(c => c.GetPropertiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobServiceProperties>>());

        var healthCheck = new StorageHealthCheck(mockClient.Object);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Equal("Storage account is accessible.", result.Description);
    }

    [Fact]
    public async Task CheckHealthAsync_ReturnsUnhealthy_WhenStorageThrows()
    {
        // Arrange
        var mockClient = new Mock<BlobServiceClient>();
        mockClient
            .Setup(c => c.GetPropertiesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException("Connection refused"));

        var healthCheck = new StorageHealthCheck(mockClient.Object);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Storage account is not accessible.", result.Description);
        Assert.IsType<RequestFailedException>(result.Exception);
    }
}
