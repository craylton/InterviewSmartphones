using InterviewSmartphones.Console.Models;
using InterviewSmartphones.Console.Services;
using Moq;
using Serilog;

namespace InterviewSmartphones.Tests;

public class LoginServiceTests
{
    private readonly Mock<ICredentialProvider> _credentialProviderMock;
    private readonly Mock<IAuthenticationClient> _authenticationClientMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly LoginService _loginService;

    public LoginServiceTests()
    {
        _credentialProviderMock = new Mock<ICredentialProvider>();
        _authenticationClientMock = new Mock<IAuthenticationClient>();
        _loggerMock = new Mock<ILogger>();
        _loginService = new LoginService(_credentialProviderMock.Object, _authenticationClientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task AuthenticateAsync_SuccessfulAuthentication_ReturnsSuccessTrue()
    {
        // Arrange
        const string username = "testuser";
        const string password = "testpass";
        var expectedResult = new LoginResult
        {
            Success = true,
            AccessToken = "test-access-token"
        };

        _credentialProviderMock.Setup(x => x.GetUsername()).Returns(username);
        _credentialProviderMock.Setup(x => x.GetPassword()).Returns(password);
        _authenticationClientMock.Setup(x => x.AuthenticateAsync(username, password, 60))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _loginService.AuthenticateAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal("test-access-token", result.AccessToken);
        _credentialProviderMock.Verify(x => x.GetUsername(), Times.Once);
        _credentialProviderMock.Verify(x => x.GetPassword(), Times.Once);
        _authenticationClientMock.Verify(x => x.AuthenticateAsync(username, password, 60), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_FailedAuthentication_ReturnsSuccessFalse()
    {
        // Arrange
        const string username = "wronguser";
        const string password = "wrongpass";
        var expectedResult = new LoginResult
        {
            Success = false,
            AccessToken = string.Empty
        };

        _credentialProviderMock.Setup(x => x.GetUsername()).Returns(username);
        _credentialProviderMock.Setup(x => x.GetPassword()).Returns(password);
        _authenticationClientMock.Setup(x => x.AuthenticateAsync(username, password, 60))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _loginService.AuthenticateAsync();

        // Assert
        Assert.False(result.Success);
        Assert.Equal(string.Empty, result.AccessToken);
        _credentialProviderMock.Verify(x => x.GetUsername(), Times.Once);
        _credentialProviderMock.Verify(x => x.GetPassword(), Times.Once);
        _authenticationClientMock.Verify(x => x.AuthenticateAsync(username, password, 60), Times.Once);
    }
}
