using System.Net.Http.Headers;
using System.Text.Json;

internal class LoginService(HttpClient client, string baseUrl)
{
    private readonly string _baseUrl = baseUrl;

    public async Task<LoginResult> AuthenticateAsync()
    {
        Console.WriteLine("Enter username:");
        var username = Console.ReadLine();

        Console.WriteLine("Enter password:");
        var password = Console.ReadLine();

        var loginRequest = new LoginRequest
        {
            Username = username!,
            Password =password!,
            ExpiresInMins = 60
        };

        var loginContent = new StringContent(JsonSerializer.Serialize(loginRequest));
        loginContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var loginResponse = await client.PostAsync($"{_baseUrl}/auth/login", loginContent);
        var loginResponseBody = await loginResponse.Content.ReadAsStringAsync();

        if (!loginResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Login failed.");
            return new LoginResult { Success = false };
        }

        var loginResult = JsonSerializer.Deserialize<LoginResponse>(loginResponseBody);

        if (string.IsNullOrEmpty(loginResult?.AccessToken))
        {
            Console.WriteLine("Login failed: Invalid response.");
            return new LoginResult { Success = false };
        }

        return new LoginResult
        {
            AccessToken = loginResult?.AccessToken!,
            Success = true
        };
    }
}
