using System;

namespace Symbiote.Net
{
    public interface IHttpAuthChallenger
    {
        void ChallengeClient(IHttpClient client, Action<bool, HttpContext> onCompletion);
    }
}