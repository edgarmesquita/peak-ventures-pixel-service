using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PeakVentures.UserTrack.PixelService.Application.Producers;

namespace PeakVentures.UserTrack.PixelService.Application.Services;

public interface IUserService
{
    void Track();
}
public class UserService : IUserService
{
    private readonly ITrackProducer _producer;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserService> _logger;

    public UserService(
        ITrackProducer producer,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserService> logger)
    {
        _producer = producer;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }
    
    public void Track()
    {
        var ipAddress = GetIpAddress() ?? GetClientIpAddress();
        if (string.IsNullOrEmpty(ipAddress))
        {
            _logger.LogInformation("The IP address cannot be collected");
            return;
        }

        var referrer = _httpContextAccessor.HttpContext?.Request.Headers.Referer.ToString();
        var userAgent = _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();
        var date = DateTime.Now.ToUniversalTime().ToString("o");
        var value = $"{date}|{referrer ?? "null"}|{userAgent ?? "null"}|{ipAddress}";
        
        _producer.Produce(value);
    }
    
    private string? GetClientIpAddress()
    {
        return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    }
    
    private string? GetIpAddress()
    {
        var ipAddress = _httpContextAccessor.HttpContext?.GetServerVariable("HTTP_X_FORWARDED_FOR");
        var remoteAddr = _httpContextAccessor.HttpContext?.GetServerVariable("REMOTE_ADDR");
    
        if (string.IsNullOrEmpty(ipAddress)) 
            return remoteAddr;
    
        var addresses = ipAddress.Split(',');
        return addresses.Length != 0 ? addresses[0] : remoteAddr;
    }
}