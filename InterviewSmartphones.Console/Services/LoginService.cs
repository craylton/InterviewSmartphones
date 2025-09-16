using Serilog;
using System.Net.Http.Headers;
using System.Text.Json;

internal class LoginService(HttpClient client, string baseUrl, ILogger logger)
{
    private readonly string _baseUrl = baseUrl;
    private readonly ILogger _logger = logger;

    public async Task<LoginResult> AuthenticateAsync()
    {
        _logger.Information("Starting authentication process");

        Console.WriteLine("Enter username:");
        var username = Console.ReadLine();

        Console.WriteLine("Enter password:");
        var password = Console.ReadLine();

        _logger.Information("Attempting login for user: {Username}", username);

        var loginRequest = new LoginRequest
        {
            Username = username!,
            Password = password!,
            ExpiresInMins = 60
        };

        var loginContent = new StringContent(JsonSerializer.Serialize(loginRequest));
        loginContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        try
        {
            var loginResponse = await client.PostAsync($"{_baseUrl}/auth/login", loginContent);
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync();

            if (!loginResponse.IsSuccessStatusCode)
            {
                _logger.Warning("Login failed with status code: {StatusCode}", loginResponse.StatusCode);
                Console.WriteLine("Login failed.");
                return new LoginResult { Success = false };
            }

            var loginResult = JsonSerializer.Deserialize<LoginResponse>(loginResponseBody);

            if (string.IsNullOrEmpty(loginResult?.AccessToken))
            {
                _logger.Warning("Login failed: Invalid response - no access token received");
                Console.WriteLine("Login failed: Invalid response.");
                return new LoginResult { Success = false };
            }

            _logger.Information("Login successful for user: {Username}", username);
            return new LoginResult
            {
                AccessToken = loginResult?.AccessToken!,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception occurred during authentication for user: {Username}", username);
            Console.WriteLine("Login failed due to an error.");
            return new LoginResult { Success = false };
        }
    }
}
