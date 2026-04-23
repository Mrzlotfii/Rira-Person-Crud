namespace SharedKernel.Infrastructure.Helper;

public class SnowflakeGeneratorOptions
{
    public int GeneratorId { get; set; } = 1;
    public DateTime Epoch { get; set; } = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public byte TimestampBits { get; set; } = 41;
    public byte GeneratorIdBits { get; set; } = 10;
    public byte SequenceBits { get; set; } = 12;
}
