using Config;
using Domain.BinaryFile;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.FetchBinaryService;

public class FetchBinaryService: IFetchBinaryService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FetchBinaryService> _logger;
    private readonly UpstreamConfig _upstreamConfig;
    
    public FetchBinaryService(IHttpClientFactory httpClientFactory, ILogger<FetchBinaryService> logger, IOptionsSnapshot<UpstreamConfig> upstremConfig)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _upstreamConfig = upstremConfig.Value;
    }
    
    public async Task<FileGetWrapper> FetchNodeBinary(string version, string flavor)
    {
        using HttpClient client = _httpClientFactory.CreateClient();
        string uri = _upstreamConfig.UpstreamBaseUri + version+ "/" + flavor;
        bool err = false;
        int currentRetry = 0;
        do
        {
            try
            {
                currentRetry += 1;
                err = false;
                var bytes = await FetchBinary(uri, client);
                
                return new FileGetWrapper()
                {
                    FileContent = bytes,
                    flavor = flavor,
                    version = version
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Something went wrong while fetching Node.JS binary from: "+uri);
                err = true;
            }
            _logger.LogDebug("Retrying");
        } while (err && currentRetry <= _upstreamConfig.AmountRetries);
        
        throw new Exception("Something went wrong while fetching Node.JS binary from: "+uri);
    }

    private async Task<byte[]> FetchBinary(string uri, HttpClient client)
    {
        var response = await client.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
        {
            string errMessage = "GET binary failed with status code: " + response.StatusCode;
            _logger.LogError(errMessage);
            throw new Exception(errMessage);
        }
        return await response.Content.ReadAsByteArrayAsync();
    }
}