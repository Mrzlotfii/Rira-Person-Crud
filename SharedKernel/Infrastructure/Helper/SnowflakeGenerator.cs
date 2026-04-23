using IdGen;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Domain;

namespace SharedKernel.Infrastructure.Helper;

public sealed class SnowflakeGenerator : ISnowflakeGenerator
{
    private readonly IdGenerator _idGen;
    private readonly ILogger<SnowflakeGenerator> _logger;

    public SnowflakeGenerator(
        IOptions<SnowflakeGeneratorOptions> options,
        ILogger<SnowflakeGenerator> logger)
    {
        ArgumentNullException.ThrowIfNull(options);

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var config = options.Value;

        if (config.TimestampBits + config.GeneratorIdBits + config.SequenceBits != 63)
        {
            throw new ArgumentException("Sum of TimestampBits, GeneratorIdBits, and SequenceBits must equal 63.");
        }

        try
        {
            var structure = new IdStructure(
                config.TimestampBits,
                config.GeneratorIdBits,
                config.SequenceBits);

            var idGenOptions = new IdGeneratorOptions(
                timeSource: new DefaultTimeSource(config.Epoch),
                idStructure: structure,
                sequenceOverflowStrategy: SequenceOverflowStrategy.SpinWait);

            _idGen = new IdGenerator(config.GeneratorId, idGenOptions);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to initialize SnowflakeGenerator. Configuration: TimestampBits={TimestampBits}, GeneratorIdBits={GeneratorIdBits}, SequenceBits={SequenceBits}, Error: {ErrorMessage}",
                config.TimestampBits, config.GeneratorIdBits, config.SequenceBits, ex.Message);
            throw new InvalidOperationException("Failed to initialize SnowflakeGenerator.", ex);
        }
    }

    public long Generate()
    {
        try
        {
            var id = _idGen.CreateId();
            _logger.LogTrace("Generated Snowflake ID: {Id}", id);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to generate Snowflake ID. System unable to generate unique identifiers, Error: {ErrorMessage}", ex.Message);
            throw new InvalidOperationException("Unable to generate a Snowflake ID.", ex);
        }
    }

    public static DateTime ExtractTimestamp(long snowflakeId, DateTime epoch, int generatorIdBits, int sequenceBits)
    {
        var timestampShift = generatorIdBits + sequenceBits;
        var timestamp = snowflakeId >> timestampShift;
        return epoch.AddMilliseconds(timestamp);
    }
}