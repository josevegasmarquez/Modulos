using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using Modulos.Client.Controllers;
using System.Net;
using System.Text.Json;
using Xunit;

namespace Modulos.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _controller = new AccountController(_mockHttpClientFactory.Object);
            
            // Setup a mock HttpContext for session
            var mockSession = new Mock<ISession>();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = mockSession.Object;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public void Login_ReturnsView()
        {
            // Act
            var result = _controller.Login();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_Post_ReturnsViewWithModelError_WhenInvalid()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.BadRequest,
                   Content = new StringContent(JsonSerializer.Serialize(new { success = false, message = "Invalid" })),
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/api/")
            };

            _mockHttpClientFactory.Setup(_ => _.CreateClient("ModulosAPI")).Returns(httpClient);

            // Act
            var result = await _controller.Login("test@example.com", "wrongpassword", false);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
        }
    }
}
