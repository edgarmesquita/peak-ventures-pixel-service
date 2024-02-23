using Microsoft.Extensions.DependencyInjection;
using PeakVentures.UserTrack.PixelService.Application.Options;
using PeakVentures.UserTrack.PixelService.Application.Producers;
using PeakVentures.UserTrack.PixelService.Application.Services;

namespace PeakVentures.UserTrack.PixelService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, Action<ApplicationOptions> options)
    {
        var appOptions = new ApplicationOptions();
        options.Invoke(appOptions);

        services.AddSingleton(appOptions);
        services.AddTransient<ITrackProducer, TrackProducer>();
        services.AddTransient<IUserService, UserService>();
        
        return services;
    }
}