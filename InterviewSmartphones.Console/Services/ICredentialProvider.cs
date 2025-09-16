namespace InterviewSmartphones.Console.Services;

public interface ICredentialProvider
{
    string GetUsername();
    string GetPassword();
}