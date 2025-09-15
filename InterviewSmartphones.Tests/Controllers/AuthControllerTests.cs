using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InterviewSmartphones.Controllers;
using InterviewSmartphones.Models;
using InterviewSmartphones.Services;

namespace InterviewSmartphones.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IDummyJsonService> _mockDummyJsonService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockDummyJsonService = new Mock<IDummyJsonService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_mockDummyJsonService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "testuser", Password = "testpass" };
            var expectedResponse = new ApiResponse<LoginResponse>
            {
                Success = true,
                Data = new LoginResponse
                {
                    Id = 1,
                    Username = "testuser",
                    Token = "test-token"
                },
                Message = "Login successful"
            };

            _mockDummyJsonService
                .Setup(s => s.LoginAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<LoginResponse>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("testuser", response.Data?.Username);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorizedResult()
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = "invalid", Password = "invalid" };
            var expectedResponse = new ApiResponse<LoginResponse>
            {
                Success = false,
                Data = null,
                Message = "Login failed",
                Errors = new List<string> { "Invalid credentials" }
            };

            _mockDummyJsonService
                .Setup(s => s.LoginAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<LoginResponse>>(unauthorizedResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Login failed", response.Message);
        }

        [Theory]
        [InlineData("", "password")]
        [InlineData("username", "")]
        [InlineData(null, "password")]
        [InlineData("username", null)]
        public async Task Login_WithMissingCredentials_ReturnsBadRequest(string username, string password)
        {
            // Arrange
            var loginRequest = new LoginRequest { Username = username, Password = password };

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<LoginResponse>>(badRequestResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Username and password are required", response.Message);
        }

        [Fact]
        public async Task GetUsers_ReturnsOkResult()
        {
            // Arrange
            var expectedResponse = new ApiResponse<List<User>>
            {
                Success = true,
                Data = new List<User>(),
                Message = "Users endpoint available"
            };

            _mockDummyJsonService
                .Setup(s => s.GetUsersAsync())
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<List<User>>>(okResult.Value);
            Assert.True(response.Success);
        }
    }
}