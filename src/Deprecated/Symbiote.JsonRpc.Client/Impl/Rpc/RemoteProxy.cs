/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Security;
using Symbiote.Core.Extensions;
using Symbiote.Core.Serialization;
using Symbiote.JsonRpc.Client.Config;

namespace Symbiote.JsonRpc.Client.Impl.Rpc
{
    public class RemoteProxy<T> : IRemoteProxy<T> where T : class
    {
        protected IJsonRpcClientConfiguration Configuration { get; set; }

        public void Call(Expression<Action<T>> call)
        {
            var methodExpression = call.Body as MethodCallExpression;
            GetResponse(methodExpression);
        }

        public R Call<R>(Expression<Func<T,R>> call)
        {
            var methodExpression = call.Body as MethodCallExpression;
            var result = GetResponse(methodExpression);
            return result.FromJson<R>();
        }

        protected string GetResponse(MethodCallExpression expression)
        {
            var request = WebRequest.Create(GetAddress(expression));
            request.Method = "POST";
            request.AuthenticationLevel = AuthenticationLevel.None;
            request.Timeout = Configuration.Timeout;

            var body = expression.GetJsonForArguments();
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

        protected string GetAddress(MethodCallExpression expression)
        {
            return @"{0}/{1}/{2}".AsFormat(Configuration.ServerUrl, typeof(T).Name, expression.Method.Name);
        }

        public RemoteProxy(IJsonRpcClientConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}