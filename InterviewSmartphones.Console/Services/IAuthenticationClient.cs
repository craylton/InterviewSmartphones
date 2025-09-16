using InterviewSmartphones.Console.Models;

namespace InterviewSmartphones.Console.Services;

public interface IAuthenticationClient
{
    Task<LoginResult> AuthenticateAsync(string username, string password, int expiresInMins = 60);
}