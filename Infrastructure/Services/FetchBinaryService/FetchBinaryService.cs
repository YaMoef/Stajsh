using Config;
using Domain.BinaryFile;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.FetchBinaryService;

public class FetchBinaryService: IFetchBinaryService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FetchBinaryService> _logger;
    private readonly UpstreamConfig _upstreamConfig;

    public FetchBinaryService(IHttpClientFactory httpClientFactory, ILogger<FetchBinaryService> logger, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _upstreamConfig = config.GetSection("UpstreamConfig").Get<UpstreamConfig>();
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
                var response = await client.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    string errMessage = "GET binary failed with status code: " + response.StatusCode;
                    _logger.LogError(errMessage);
                    throw new Exception(errMessage);
                } 
                
                return new FileGetWrapper()
                {
                    FileContent = await response.Content.ReadAsByteArrayAsync(),
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
}