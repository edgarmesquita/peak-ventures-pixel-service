using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using PeakVentures.UserTrack.PixelService.Application.Producers;
using PeakVentures.UserTrack.PixelService.Application.Services;

namespace PeakVentures.UserTrack.PixelService.Application.Tests.Services;

[TestFixture]
    public class UserServiceTests
    {
        [Test]
        public void Track_WithRemoteAddr_SuccessfullyTracksUser()
        {
            // Arrange
            var producerMock = new Mock<ITrackProducer>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var loggerMock = new Mock<ILogger<UserService>>();
            var variablesFeature = new Mock<IServerVariablesFeature>();
            variablesFeature.Setup(o => o[It.IsAny<string>()]).Returns("test");
            var featureCollection = new FeatureCollection();
            featureCollection.Set(variablesFeature.Object );
            var defaultContext = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            var headers = new HeaderDictionary(new Dictionary<string, StringValues>
            {
                { HeaderNames.UserAgent, "UserAgent" },
                { HeaderNames.Referer, "http://localhost" }
            });
            httpRequestMock.Setup(x => x.Headers).Returns(headers);
            defaultContext.Setup(o => o.Request).Returns(httpRequestMock.Object);
            defaultContext.Setup(o => o.Features).Returns(featureCollection);
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(defaultContext.Object);
            
            var userService = new UserService(
                producerMock.Object,
                httpContextAccessorMock.Object,
                loggerMock.Object);

            // Act
            userService.Track();

            // Assert
            producerMock.Verify(p => p.Produce(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Track_NoIpAddress_ProduceNothing()
        {
            // Arrange
            var producerMock = new Mock<ITrackProducer>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var loggerMock = new Mock<ILogger<UserService>>();
            
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            var userService = new UserService(
                producerMock.Object,
                httpContextAccessorMock.Object,
                loggerMock.Object);

            // Act
            userService.Track();

            // Assert
            producerMock.Verify(p => p.Produce(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Track_WithIpAddress_SuccessfullyTracksUser()
        {
            // Arrange
            var producerMock = new Mock<ITrackProducer>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var loggerMock = new Mock<ILogger<UserService>>();
            
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                Connection = { RemoteIpAddress = new System.Net.IPAddress(123456789) }
            });

            var userService = new UserService(
                producerMock.Object,
                httpContextAccessorMock.Object,
                loggerMock.Object);

            // Act
            userService.Track();

            // Assert
            producerMock.Verify(p => p.Produce(It.IsAny<string>()), Times.Once);
        }
    }