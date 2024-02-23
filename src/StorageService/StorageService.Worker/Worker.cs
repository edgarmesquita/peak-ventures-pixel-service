using System.Text;
using Confluent.Kafka;
using PeakVentures.UserTrack.StorageService.Worker.Services;
using PeakVentures.UserTrack.StorageService.Worker.Settings;

namespace PeakVentures.UserTrack.StorageService.Worker;

public class Worker : BackgroundService
{
    private readonly AppSettings _settings;
    private readonly IFileService _fileService;
    private readonly ILogger<Worker> _logger;
    private const string Topic = "track";
    
    public Worker(
        AppSettings settings, 
        IFileService fileService,
        ILogger<Worker> logger)
    {
        _settings = settings;
        _fileService = fileService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(_settings.Confluent.KafkaClientConfig).Build();
        consumer.Subscribe(Topic);
        while (!stoppingToken.IsCancellationRequested) {
            var cr = consumer.Consume();
            await _fileService.WriteLineToLogFileAsync(cr.Message.Value);
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Consumed event from topic {Topic}: key = {MessageKey} value = {MessageValue}", Topic, cr.Message.Key, cr.Message.Value);
            }
        }
        consumer.Close();
    }
}