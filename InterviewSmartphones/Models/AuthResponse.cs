using System.Text.Json.Serialization;

namespace InterviewSmartphones.Models;

public class AuthResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;
}
