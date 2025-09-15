using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using InterviewSmartphones.Controllers;
using InterviewSmartphones.Models;
using InterviewSmartphones.Services;

namespace InterviewSmartphones.Tests.Controllers
{
    public class SmartphonesControllerTests
    {
        private readonly Mock<IDummyJsonService> _mockDummyJsonService;
        private readonly Mock<ILogger<SmartphonesController>> _mockLogger;
        private readonly SmartphonesController _controller;

        public SmartphonesControllerTests()
        {
            _mockDummyJsonService = new Mock<IDummyJsonService>();
            _mockLogger = new Mock<ILogger<SmartphonesController>>();
            _controller = new SmartphonesController(_mockDummyJsonService.Object, _mockLogger.Object);

            // Setup HTTP context with authorization header
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer test-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task GetTopThreeSmartphones_WithValidToken_ReturnsOkResult()
        {
            // Arrange
            var expectedSmartphones = new List<SmartphoneDto>
            {
                new SmartphoneDto { Id = 1, Title = "iPhone 15", Brand = "Apple", Price = 999 },
                new SmartphoneDto { Id = 2, Title = "Samsung Galaxy S24", Brand = "Samsung", Price = 899 },
                new SmartphoneDto { Id = 3, Title = "Google Pixel 8", Brand = "Google", Price = 799 }
            };

            var expectedResponse = new ApiResponse<List<SmartphoneDto>>
            {
                Success = true,
                Data = expectedSmartphones,
                Message = "Found 3 smartphones"
            };

            _mockDummyJsonService
                .Setup(s => s.GetTopThreeSmartphonesAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetTopThreeSmartphones();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<List<SmartphoneDto>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(3, response.Data?.Count);
        }

        [Fact]
        public async Task GetTopThreeSmartphones_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            // No authorization header
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _controller.GetTopThreeSmartphones();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<List<SmartphoneDto>>>(unauthorizedResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Authorization token is required", response.Message);
        }

        [Fact]
        public async Task UpdateSmartphonePrices_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var priceUpdateRequest = new PriceUpdateRequest { PercentageIncrease = 10 };
            var originalSmartphones = new List<SmartphoneDto>
            {
                new SmartphoneDto { Id = 1, Title = "iPhone 15", Brand = "Apple", Price = 999 },
                new SmartphoneDto { Id = 2, Title = "Samsung Galaxy S24", Brand = "Samsung", Price = 899 }
            };

            var updatedSmartphones = new List<SmartphoneDto>
            {
                new SmartphoneDto { Id = 1, Title = "iPhone 15", Brand = "Apple", Price = 1098.9m, OriginalPrice = 999 },
                new SmartphoneDto { Id = 2, Title = "Samsung Galaxy S24", Brand = "Samsung", Price = 988.9m, OriginalPrice = 899 }
            };

            _mockDummyJsonService
                .Setup(s => s.GetTopThreeSmartphonesAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApiResponse<List<SmartphoneDto>>
                {
                    Success = true,
                    Data = originalSmartphones
                });

            _mockDummyJsonService
                .Setup(s => s.UpdateProductPriceAsync(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .ReturnsAsync((int id, decimal price, string token) => new ApiResponse<SmartphoneDto>
                {
                    Success = true,
                    Data = updatedSmartphones.First(s => s.Id == id)
                });

            // Act
            var result = await _controller.UpdateSmartphonePrices(priceUpdateRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<List<SmartphoneDto>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(2, response.Data?.Count);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1001)]
        public async Task UpdateSmartphonePrices_WithInvalidPercentage_ReturnsBadRequest(decimal percentage)
        {
            // Arrange
            var priceUpdateRequest = new PriceUpdateRequest { PercentageIncrease = percentage };

            // Act
            var result = await _controller.UpdateSmartphonePrices(priceUpdateRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<List<SmartphoneDto>>>(badRequestResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Invalid percentage increase", response.Message);
        }

        [Fact]
        public async Task UpdateSmartphonePrices_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            // No authorization header
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var priceUpdateRequest = new PriceUpdateRequest { PercentageIncrease = 10 };

            // Act
            var result = await _controller.UpdateSmartphonePrices(priceUpdateRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<List<SmartphoneDto>>>(unauthorizedResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Authorization token is required", response.Message);
        }
    }
}