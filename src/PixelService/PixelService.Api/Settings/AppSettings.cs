namespace PeakVentures.PixelService.Api.Settings;

public class AppSettings
{
    public ConfluentSettings Confluent { get; set; } = new();
}

public class ConfluentSettings
{
    public Dictionary<string, string> KafkaClientConfig { get; set; } = new();
}