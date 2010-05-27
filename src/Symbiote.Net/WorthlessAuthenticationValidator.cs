namespace Symbiote.Net
{
    public class WorthlessAuthenticationValidator : IAuthenticationValidator
    {
        public bool ValidateCredentials(string username, string password)
        {
            return true;
        }

        public bool ValidateToken(string token)
        {
            return true;
        }
    }
}