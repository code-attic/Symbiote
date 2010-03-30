using System;

namespace Symbiote.Relax.Impl
{
    public interface ICouchCommand
    {
        string GetResponse(CouchUri uri, string method, string body);
        void GetContinuousResponse(CouchUri uri, int since, Action<ChangeRecord> callback);
        void StopContinousResponse();
        string Post(CouchUri uri);
        string Post(CouchUri uri, string body);
        string Put(CouchUri uri);
        string Put(CouchUri uri, string body);
        string Get(CouchUri uri);
        string Delete(CouchUri uri);
    }
}