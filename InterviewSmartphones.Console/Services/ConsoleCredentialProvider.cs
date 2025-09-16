namespace InterviewSmartphones.Console.Services;

public class ConsoleCredentialProvider : ICredentialProvider
{
    public string GetUsername()
    {
        System.Console.WriteLine("Enter username:");
        return System.Console.ReadLine() ?? string.Empty;
    }

    public string GetPassword()
    {
        System.Console.WriteLine("Enter password:");
        return System.Console.ReadLine() ?? string.Empty;
    }
}