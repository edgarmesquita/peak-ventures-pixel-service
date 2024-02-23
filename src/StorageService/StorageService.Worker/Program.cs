using PeakVentures.UserTrack.StorageService.Worker;
using PeakVentures.UserTrack.StorageService.Worker.Services;
using PeakVentures.UserTrack.StorageService.Worker.Settings;

var builder = Host.CreateApplicationBuilder(args);
var settings = new AppSettings();
builder.Configuration.Bind(settings);
builder.Services.AddSingleton(settings);
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();