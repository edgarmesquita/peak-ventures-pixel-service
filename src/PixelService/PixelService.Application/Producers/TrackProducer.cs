using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using PeakVentures.UserTrack.PixelService.Application.Options;

namespace PeakVentures.UserTrack.PixelService.Application.Producers;

public interface ITrackProducer
{
    void Produce(string value);
}
public class TrackProducer : ITrackProducer
{
    private readonly ApplicationOptions _options;
    private readonly ILogger<TrackProducer> _logger;
    private const string Topic = "track";

    public TrackProducer(
        ApplicationOptions options,
        ILogger<TrackProducer> logger)
    {
        _options = options;
        _logger = logger;
    }

    public void Produce(string value)
    {
        using var producer = new ProducerBuilder<string, string>(_options.KafkaClientConfig).Build();
        producer.Produce(Topic, new Message<string, string> { Key = Guid.NewGuid().ToString(), Value = value },
            (deliveryReport) =>
            {
                if (deliveryReport.Error.Code != ErrorCode.NoError)
                {
                    _logger.LogError("Failed to deliver message: {ErrorReason}", deliveryReport.Error.Reason);
                }
                else
                {
                    _logger.LogInformation("Produced event to topic {Topic}: key = {MessageKey} value = {MessageValue}",
                        Topic, deliveryReport.Message.Key, deliveryReport.Message.Value);
                }
            }
        );
        producer.Flush(TimeSpan.FromSeconds(10));
    }
}