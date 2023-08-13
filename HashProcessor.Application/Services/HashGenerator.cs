using System.Security.Cryptography;
using System.Text;

namespace HashProcessor.Application.Services;

public class HashGenerator : IHashGenerator
{
    public List<byte[]> GenerateHashes(int count)
    {
        var salt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString() + DateTime.UtcNow.Ticks);

        var iterationElements = Enumerable.Range(0, count);
        var hashes = new List<byte[]>();

        Parallel.ForEach(
            iterationElements,
            () => new List<byte[]>(), // initialized once per a thread
            (elem, _, _, dataState) =>
            {
                var indexInBytes = BitConverter.GetBytes(elem);
                var hashInput = new byte[salt.Length + indexInBytes.Length];
                Array.Copy(salt, hashInput, salt.Length);
                Array.Copy(indexInBytes, 0, hashInput, salt.Length, indexInBytes.Length);

                // Hash algorithm selection can be extracted and injected via DI
                var hashBytes = SHA1.HashData(hashInput);
                dataState.Add(hashBytes);

                return dataState;
            },
            data =>
            {
                lock (hashes) // may be accessed by multiple threads so need an access synchronization mechanism
                {
                    hashes.AddRange(data);
                }
            });

        return hashes;
    }
}
