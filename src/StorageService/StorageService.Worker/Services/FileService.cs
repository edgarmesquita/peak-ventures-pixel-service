using System.Text;
using PeakVentures.UserTrack.StorageService.Worker.Settings;

namespace PeakVentures.UserTrack.StorageService.Worker.Services;

public interface IFileService
{
    Task WriteLineToLogFileAsync(string line);
}

public class FileService : IFileService
{
    private readonly AppSettings _settings;
    private readonly ILogger<FileService> _logger;

    public FileService(AppSettings settings, ILogger<FileService> logger)
    {
        _settings = settings;
        _logger = logger;
    }
    
    public async Task WriteLineToLogFileAsync(string line)
    {
        try
        {
            await CreateLogFileIfNotExistsAsync();

            await using var file = new FileStream(_settings.Storage.LogFilePath, FileMode.Append, FileAccess.Write,
                FileShare.Read);
            await using var writer = new StreamWriter(file, Encoding.Unicode);
            await writer.WriteLineAsync(line);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when creating/changing log file");
        }
    }

    private async Task CreateLogFileIfNotExistsAsync()
    {
        if (!File.Exists(_settings.Storage.LogFilePath))
        {
            await using (File.Create(_settings.Storage.LogFilePath)) {}
        }
    }
}