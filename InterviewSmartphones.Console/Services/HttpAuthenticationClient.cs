using System.Net.Http.Headers;
using System.Text.Json;
using InterviewSmartphones.Console.Models;
using Serilog;

namespace InterviewSmartphones.Console.Services;

public class HttpAuthenticationClient(HttpClient client, string baseUrl, ILogger logger) : IAuthenticationClient
{
    public async Task<LoginResult> AuthenticateAsync(string username, string password, int expiresInMins = 60)
    {
        logger.Information("Attempting authentication for user: {Username}", username);

        var loginRequest = new LoginRequest
        {
            Username = username,
            Password = password,
            ExpiresInMins = expiresInMins
        };

        var loginContent = new StringContent(JsonSerializer.Serialize(loginRequest));
        loginContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        try
        {
            var loginResponse = await client.PostAsync($"{baseUrl}/auth/login", loginContent);
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync();

            if (!loginResponse.IsSuccessStatusCode)
            {
                logger.Warning("Authentication failed with status code: {StatusCode}", loginResponse.StatusCode);
                return new LoginResult { Success = false };
            }

            var loginResult = JsonSerializer.Deserialize<LoginResponse>(loginResponseBody);

            if (string.IsNullOrEmpty(loginResult?.AccessToken))
            {
                logger.Warning("Authentication failed: Invalid response - no access token received");
                return new LoginResult { Success = false };
            }

            logger.Information("Authentication successful for user: {Username}", username);
            return new LoginResult
            {
                AccessToken = loginResult.AccessToken,
                Success = true
            };
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Exception occurred during authentication for user: {Username}", username);
            return new LoginResult { Success = false };
        }
    }
}