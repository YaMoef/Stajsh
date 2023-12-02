using System.Configuration;
using System.Net;
using Config;
using Infrastructure.Services.FetchBinaryService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using UnitTests.Helpers;

namespace UnitTests.Services;

[TestClass]
public class FetchBinaryServiceTest
{
    private const string baseUrl = "https://nodejs.org/dis/";
    private const int amountRetries = 5;
    private const string version = "v19.8.0";
    private const string flavor = "node-v19.8.0-aix-ppc64.tar.gz";
    
    [TestMethod]
    public async Task ShouldFetchNodePackage()
    {
        // Arrange
        // Set up mocks
        var loggerMock = Substitute.For<ILogger<FetchBinaryService>>();
        var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
        var upstreamConfigSnapshotMock = Substitute.For<IOptionsSnapshot<UpstreamConfig>>();
        
        // Setting up mocks and services
        var httpMessageHandlerMock = new MockHttpMessageHandler("", HttpStatusCode.OK);
        var httpClient = new HttpClient(httpMessageHandlerMock);
        
        upstreamConfigSnapshotMock.Value.Returns(new UpstreamConfig()
            { AmountRetries = amountRetries, UpstreamBaseUri = baseUrl });
        httpClientFactoryMock.CreateClient().Returns(httpClient);
        var fetchBinaryService = new FetchBinaryService(httpClientFactoryMock, loggerMock, upstreamConfigSnapshotMock);

        // Act
        var fileWrapper = await fetchBinaryService.FetchNodeBinary(version, flavor);

        // Assert
        Assert.IsNotNull(fileWrapper);
    }
    
    [TestMethod]
    public async Task ShouldThrowErrorOnBadGateWayAfterRetries()
    {
        // Arrange
        // Set up mocks
        var loggerMock = Substitute.For<ILogger<FetchBinaryService>>();
        var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
        var upstreamConfigSnapshotMock = Substitute.For<IOptionsSnapshot<UpstreamConfig>>();
        
        // Setting up mocks and services
        var httpMessageHandlerMock = new MockHttpMessageHandler("", HttpStatusCode.BadGateway);
        var httpClient = new HttpClient(httpMessageHandlerMock);
        
        upstreamConfigSnapshotMock.Value.Returns(new UpstreamConfig()
            { AmountRetries = amountRetries, UpstreamBaseUri = baseUrl });
        httpClientFactoryMock.CreateClient().Returns(httpClient);
        var fetchBinaryService = new FetchBinaryService(httpClientFactoryMock, loggerMock, upstreamConfigSnapshotMock);

        // Act and Assert
        Assert.ThrowsExceptionAsync<Exception>(async () => await fetchBinaryService.FetchNodeBinary(version, flavor));
    }
    
}