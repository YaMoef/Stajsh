namespace Config;

public sealed class UpstreamConfig
{
    public required int AmountRetries { get; set; }

    public required string UpstreamBaseUri { get; set; }
}