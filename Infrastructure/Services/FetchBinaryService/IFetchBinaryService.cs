using Domain.BinaryFile;

namespace Infrastructure.Services.FetchBinaryService;

public interface IFetchBinaryService
{
    public Task<FileGetWrapper> FetchNodeBinary(string version, string flavor);
}