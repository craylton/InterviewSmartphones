using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using InterviewSmartphones.Models;

namespace InterviewSmartphones.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<AuthController> logger) : ControllerBase
{
    private readonly string _baseUrl = configuration["BaseUrl"] ?? "https://dummyjson.com";
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Username and password are required");
        }

        var loginPayload = new
        {
            username = request.Username,
            password = request.Password,
            expiresInMins = 60
        };

        var loginContent = new StringContent(JsonSerializer.Serialize(loginPayload));
        loginContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        _logger.LogInformation("Sending login request...");

        try
        {
            var loginResponse = await _httpClient.PostAsync($"{_baseUrl}/auth/login", loginContent);
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync();

            _logger.LogInformation($"Login Status Code: {loginResponse.StatusCode}");
            _logger.LogInformation($"Login Response Body: {loginResponseBody}");

            if (!loginResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Login failed.");
                return Unauthorized("Login failed");
            }

            var loginResult = JsonSerializer.Deserialize<AuthResponse>(loginResponseBody);

            if (loginResult?.AccessToken == null)
            {
                _logger.LogError("Invalid response from authentication server.");
                return BadRequest("Invalid response from authentication server");
            }

            return Ok(loginResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication");
            return StatusCode(500, "Internal server error during authentication");
        }
    }
}