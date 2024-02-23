using Microsoft.AspNetCore.Mvc;
using PeakVentures.PixelService.Api.Settings;
using PeakVentures.UserTrack.PixelService.Application.Extensions;
using PeakVentures.UserTrack.PixelService.Application.Producers;
using PeakVentures.UserTrack.PixelService.Application.Services;

var builder = WebApplication.CreateBuilder(args);
var settings = new AppSettings();
builder.Configuration.Bind(settings);
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplication(opt => opt.KafkaClientConfig = settings.Confluent.KafkaClientConfig);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseForwardedHeaders();

app.MapGet("/track", (
        [FromServices] IUserService service,
        [FromServices] IHttpContextAccessor httpContextAccessor
    ) =>
    {
        service.Track();
        const string trackingPixel = "R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==";
        return TypedResults.File(Convert.FromBase64String(trackingPixel), "image/gif");
    })
    .WithName("Track")
    .Produces(StatusCodes.Status200OK)
    .WithOpenApi();

app.Run();