namespace PeakVentures.UserTrack.PixelService.Application.Options;

public class ApplicationOptions
{
    public Dictionary<string, string> KafkaClientConfig { get; set; } = new();
}