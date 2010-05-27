namespace Symbiote.Net
{
    public interface IAuthenticationValidator
    {
        bool ValidateCredentials(string username, string password);
        bool ValidateToken(string token);
    }
}