using System.Net;
using System.Text;
using System.Text.Json;
using InterviewSmartphones.Console.Models;
using InterviewSmartphones.Console.Services;
using Moq;
using Moq.Protected;
using Serilog;

namespace InterviewSmartphones.Tests;

public class HttpAuthenticationClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger> _loggerMock;
    private readonly HttpAuthenticationClient _authenticationClient;
    private const string BaseUrl = "https://test-api.example.com";

    public HttpAuthenticationClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _loggerMock = new Mock<ILogger>();
        _authenticationClient = new HttpAuthenticationClient(_httpClient, BaseUrl, _loggerMock.Object);
    }

    [Fact]
    public async Task AuthenticateAsync_SuccessfulResponse_ReturnsSuccessTrue()
    {
        // Arrange
        const string username = "testuser";
        const string password = "testpass";
        const string accessToken = "test-access-token-12345";

        var successResponse = new LoginResponse { AccessToken = accessToken };
        var responseContent = JsonSerializer.Serialize(successResponse);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                It.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString() == $"{BaseUrl}/auth/login"),
                It.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _authenticationClient.AuthenticateAsync(username, password);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(accessToken, result.AccessToken);
    }

    [Fact]
    public async Task AuthenticateAsync_UnauthorizedResponse_ReturnsSuccessFalse()
    {
        // Arrange
        const string username = "wronguser";
        const string password = "wrongpass";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString() == $"{BaseUrl}/auth/login"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("{\"error\": \"Invalid credentials\"}", Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _authenticationClient.AuthenticateAsync(username, password);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(string.Empty, result.AccessToken);
    }

    [Fact]
    public async Task AuthenticateAsync_EmptyAccessToken_ReturnsSuccessFalse()
    {
        // Arrange
        const string username = "testuser";
        const string password = "testpass";

        var invalidResponse = new LoginResponse { AccessToken = string.Empty };
        var responseContent = JsonSerializer.Serialize(invalidResponse);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _authenticationClient.AuthenticateAsync(username, password);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(string.Empty, result.AccessToken);
    }
}