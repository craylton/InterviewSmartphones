using InterviewSmartphones.Console.Models;
using Serilog;

namespace InterviewSmartphones.Console.Services;

public class LoginService(ICredentialProvider credentialProvider, IAuthenticationClient authenticationClient, ILogger logger)
{
    private readonly ICredentialProvider _credentialProvider = credentialProvider;
    private readonly IAuthenticationClient _authenticationClient = authenticationClient;
    private readonly ILogger _logger = logger;

    public async Task<LoginResult> AuthenticateAsync()
    {
        _logger.Information("Starting authentication process");

        var username = _credentialProvider.GetUsername();
        var password = _credentialProvider.GetPassword();

        return await _authenticationClient.AuthenticateAsync(username, password);
    }
}
