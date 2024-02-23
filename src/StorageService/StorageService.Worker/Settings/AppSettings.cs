namespace PeakVentures.UserTrack.StorageService.Worker.Settings;

public class AppSettings
{
    public ConfluentSettings Confluent { get; set; } = new();
    public StorageSettings Storage { get; set; } = new();
}

public class ConfluentSettings
{
    public Dictionary<string, string> KafkaClientConfig { get; set; } = new();
}

public class StorageSettings
{
    public string LogFilePath { get; set; } = string.Empty;
}