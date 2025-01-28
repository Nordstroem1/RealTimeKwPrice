using API.Controllers.PriceController;
using Domain.Interfaces;
using Domain.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCollectionOrderer("Xunit.Sdk.TestCollectionOrderer", "xunit.core")]

namespace RealTimeKWhPrice.Test.ControllerTests
{
    public class ElectricityPriceControllerTests : IClassFixture<WebApplicationFactory<Presentation.Program>>
    {
        private readonly WebApplicationFactory<Presentation.Program> _factory;
        private readonly ILogger<ElectricityPriceController> _logger;
        private readonly ILoggerRepository _loggerToDatabase;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

        public ElectricityPriceControllerTests(WebApplicationFactory<Presentation.Program> factory)
        {
            _factory = factory;
            _logger = A.Fake<ILogger<ElectricityPriceController>>();
            _loggerToDatabase = A.Fake<ILoggerRepository>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        }

        [Fact]
        public async Task VerifyApiConnection_ReturnsOk()
        {
            // Arrange
            var region = "SE1";
            var url = $"https://www.elprisetjustnu.se/api/v1/prices/{DateTime.Now:yyyy}/{DateTime.Now:MM-dd}_{region.ToUpper()}.json";

            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new List<ElectricityPrice>
                {
                    new ElectricityPrice
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.NewGuid(),
                        SEK_per_kWh = 1.23M,
                        EUR_per_kWh = 0.12M,
                        EXR = 10.25M,
                        time_start = DateTime.Now,
                        time_end = DateTime.Now.AddHours(1)
                    }
                })
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == url),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(fakeResponse);

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            var controller = new ElectricityPriceController(httpClient, _logger, _loggerToDatabase);

            A.CallTo(() => _loggerToDatabase.LogErrorAsync(A<Logger>.Ignored))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.GetElectricityPrices(region) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var data = result.Value as List<ElectricityPrice>;
            Assert.NotNull(data);
            Assert.Single(data);
            Assert.Equal(1.23M, data[0].SEK_per_kWh);
        }
    }
}
