using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;
using InterviewSmartphones.Services;
using InterviewSmartphones.Models;
using Moq.Protected;

namespace InterviewSmartphones.Tests.Services
{
    public class DummyJsonServiceTests
    {
        private readonly Mock<ILogger<DummyJsonService>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly DummyJsonService _service;

        public DummyJsonServiceTests()
        {
            _mockLogger = new Mock<ILogger<DummyJsonService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _service = new DummyJsonService(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsSuccessResponse()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "testuser", Password = "testpass" };
            var expectedResponse = new LoginResponse
            {
                Id = 1,
                Username = "testuser",
                Token = "test-token",
                FirstName = "Test",
                LastName = "User"
            };

            var jsonResponse = JsonSerializer.Serialize(expectedResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _service.LoginAsync(loginRequest);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("testuser", result.Data.Username);
            Assert.Equal("test-token", result.Data.Token);
            Assert.Equal("Login successful", result.Message);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ReturnsFailureResponse()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "invalid", Password = "invalid" };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("Invalid credentials", Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _service.LoginAsync(loginRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Login failed", result.Message);
            Assert.Contains("Invalid credentials", result.Errors);
        }

        [Fact]
        public async Task LoginAsync_WithNetworkError_ReturnsErrorResponse()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "testuser", Password = "testpass" };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await _service.LoginAsync(loginRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Network error occurred", result.Message);
            Assert.Contains("Network error", result.Errors);
        }

        [Fact]
        public async Task GetTopThreeSmartphonesAsync_WithValidToken_ReturnsSmartphones()
        {
            // Arrange
            var token = "valid-token";
            var productsResponse = new ProductsResponse
            {
                Products = new List<Product>
                {
                    new Product { Id = 1, Title = "iPhone 15", Brand = "Apple", Price = 999, Category = "smartphones" },
                    new Product { Id = 2, Title = "Samsung Galaxy S24", Brand = "Samsung", Price = 899, Category = "smartphones" },
                    new Product { Id = 3, Title = "Google Pixel 8", Brand = "Google", Price = 799, Category = "smartphones" },
                    new Product { Id = 4, Title = "OnePlus 11", Brand = "OnePlus", Price = 699, Category = "smartphones" }
                }
            };

            var jsonResponse = JsonSerializer.Serialize(productsResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _service.GetTopThreeSmartphonesAsync(token);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(3, result.Data.Count);
            Assert.Equal("iPhone 15", result.Data[0].Title);
            Assert.Equal(999, result.Data[0].Price);
        }

        [Fact]
        public async Task UpdateProductPriceAsync_WithValidData_ReturnsUpdatedProduct()
        {
            // Arrange
            var productId = 1;
            var newPrice = 1099m;
            var token = "valid-token";
            var updatedProduct = new Product
            {
                Id = productId,
                Title = "iPhone 15",
                Brand = "Apple",
                Price = newPrice
            };

            var jsonResponse = JsonSerializer.Serialize(updatedProduct, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _service.UpdateProductPriceAsync(productId, newPrice, token);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(productId, result.Data.Id);
            Assert.Equal(newPrice, result.Data.Price);
            Assert.Equal("Price updated successfully", result.Message);
        }

        [Theory]
        [InlineData("", "password")]
        [InlineData("username", "")]
        [InlineData(null, "password")]
        [InlineData("username", null)]
        public async Task LoginAsync_WithInvalidInput_ShouldHandleGracefully(string username, string password)
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = username, Password = password };

            // Act & Assert - Should not throw exception
            var result = await _service.LoginAsync(loginRequest);
            
            // The service should handle the request gracefully
            Assert.NotNull(result);
        }
    }
}