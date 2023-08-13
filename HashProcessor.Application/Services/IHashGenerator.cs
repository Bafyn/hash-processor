namespace HashProcessor.Application.Services;

public interface IHashGenerator
{
    List<byte[]> GenerateHashes(int count);
}
