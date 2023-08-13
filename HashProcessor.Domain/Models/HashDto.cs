namespace HashProcessor.Domain.Models;

public class HashDto
{
    public DateTime Date { get; set; }

    public byte[][] Hashes { get; set; }
}
