using System;

namespace Symbiote.Relax.Impl
{
    public interface ICouchCommand
    {
        string GetResponse(CouchURI uri, string method, string body);
        void GetContinuousResponse(CouchURI uri, int since, Action<ChangeRecord> callback);
        void StopContinousResponse();
        string Post(CouchURI uri);
        string Post(CouchURI uri, string body);
        string Put(CouchURI uri);
        string Put(CouchURI uri, string body);
        string Get(CouchURI uri);
        string Delete(CouchURI uri);
    }
}