using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Security;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Restfully.Impl;

namespace Symbiote.Restfully
{
    public class RemoteProxy<T>
        where T : class
    {
        protected IHttpClientConfiguration Configuration { get; set; }

        public void Call(Expression<Action<T>> call)
        {
            var remoteAction = new RemoteAction<T>(call);
            GetResponse(remoteAction);
        }

        public R Call<R>(Expression<Func<T,R>> call)
        {
            var remoteFunc = new RemoteFunc<T,R>(call);
            var result = GetResponse(remoteFunc);
            return result.FromJson<R>();
        }

        protected string GetResponse(RemoteProcedure<T> remoteProcedure)
        {
            var request = WebRequest.Create(GetAddress(remoteProcedure));
            request.Method = "POST";
            request.AuthenticationLevel = AuthenticationLevel.None;
            request.Timeout = Configuration.Timeout;
            
            var body = remoteProcedure.ToJson(true);
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = body.Length;

            using (var stream = request.GetRequestStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(body);
                writer.Flush();
            }

            var result = "";
            var response = request.GetResponse();

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
                response.Close();
            }

            return result;
        }

        protected string GetAddress(RemoteProcedure<T> remoteProcedure)
        {
            return @"{0}\{1}\{2}".AsFormat(Configuration.ServerUrl, remoteProcedure.Contract, remoteProcedure.Method);
        }

        public RemoteProxy(IHttpClientConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}