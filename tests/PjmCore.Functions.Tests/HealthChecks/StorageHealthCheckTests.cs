using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using PjmCore.Functions.HealthChecks;

namespace PjmCore.Functions.Tests.HealthChecks;

public class StorageHealthCheckTests
{
    private static Mock<BlobServiceClient> CreateServiceClient(Mock<BlobContainerClient> containerClient)
    {
        var serviceClient = new Mock<BlobServiceClient>();
        serviceClient
            .Setup(c => c.GetBlobContainerClient(StorageHealthCheck.ContainerName))
            .Returns(containerClient.Object);
        return serviceClient;
    }

    [Fact]
    public async Task CheckHealthAsync_ReturnsHealthy_WhenStorageIsAccessible()
    {
        // Arrange
        var mockContainer = new Mock<BlobContainerClient>();
        mockContainer
            .Setup(c => c.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContainerProperties>>());

        var healthCheck = new StorageHealthCheck(CreateServiceClient(mockContainer).Object);

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
        var mockContainer = new Mock<BlobContainerClient>();
        mockContainer
            .Setup(c => c.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException("Connection refused"));

        var healthCheck = new StorageHealthCheck(CreateServiceClient(mockContainer).Object);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Storage account is not accessible.", result.Description);
        Assert.IsType<RequestFailedException>(result.Exception);
    }
}
