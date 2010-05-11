using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Configuration;
using System.Web.Routing;
using Machine.Specifications;
using Symbiote.Restfully;
using Machine.Specifications.Runner.Impl;
using Symbiote.Core.Reflection;

namespace Restfully.Tests
{
    public abstract class with_http_listener
    {
        protected static HttpListener listener;
        protected static HttpListenerContext httpContext;
        protected static string baseUri;
        protected static string relativePath;
        protected static string queryString;
        protected static ResourceRequest resourceRequest;

        private Establish context = () =>
                                        {
                                            baseUri = "http://localhost:8420/";
                                            listener = new HttpListener();
                                            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                                            listener.Prefixes.Add(baseUri);
                                            listener.Start();
                                            listener.BeginGetContext(ProcessRequest, null);
                                        };

        public static void ProcessRequest(IAsyncResult asyncResult)
        {
            httpContext = listener.EndGetContext(asyncResult);

            relativePath = httpContext.Request.Url.AbsolutePath;
            queryString = httpContext.Request.Url.Query;

            using(var stream = httpContext.Response.OutputStream)
            {
                var textWriter = new StreamWriter(stream);
                textWriter.Write("howdy");
                textWriter.Flush();
            }
        }
    }

    public abstract class with_web_request : with_http_listener
    {
        protected static string requestUri;
        protected static WebRequest request;

        private Establish context = () =>
                                        {
                                            requestUri = baseUri + @"controller/action/arg1/arg2?a=1&b=2";
                                            request = WebRequest.Create(requestUri);
                                        };
    }

    [Subject("simple request")]
    public class when_simple_get_request : with_web_request
    {
        protected static WebResponse response;
        protected static string result;

        private Because of = () =>
                                 {
                                     request.Method = "GET";
                                     request.ContentType = "text/plain";
                                     using (var stream = request.GetRequestStream())
                                     using (var writer = new StringWriter())
                                     {
                                         writer.WriteLine("Anyone home?");
                                         writer.Flush();
                                     }
                                     response = request.GetResponse();
                                     using (var reader = new StreamReader(response.GetResponseStream()))
                                     {
                                         result = reader.ReadToEnd();
                                     }
                                 };

        private It should_have_uri_in_context = () => httpContext.Request.Url.AbsoluteUri.ShouldEqual(requestUri);
        private It should_have_path_in_context = () => relativePath.ShouldEqual(@"/controller/action/arg1/arg2");
        private It should_have_query_string_in_context = () => queryString.ShouldEqual(@"?a=1&b=2");
        private It should_have_response = () => result.ShouldEqual("howdy");


    }

    public class ComplexReturn
    {
        public string Property1 { get; set; }
        public DateTime Date { get; set; }
        public Guid Id { get; set; }
    }

    public class ComplexArg
    {
        public string Property1 { get; set; }
        public DateTime Date { get; set; }
        public Guid Id { get; set; }
    }

    public interface  ITestService
    {
        void OneArgCall(string arg1);
        bool TwoArgCall(DateTime date, Guid id);

        ComplexReturn ComplexCall(ComplexArg arg);
    }

    public class RestfulProxy<T>
        where T : class
    {
        public string Method { get; set; }
        public Dictionary<string, object> Args { get; set; }

        public void Call(Expression<Action<T>> call)
        {
            var methodCallExpression = call.Body as MethodCallExpression;

            Method = methodCallExpression.Method.Name;

            var arguments = methodCallExpression.Arguments;

            var values = arguments
                .Cast<MemberExpression>()
                .Select(x => GetExpressionValue(x))
                .ToArray();

            int valueIndex = 0;
            Args = methodCallExpression
                .Method
                .GetParameters()
                .ToDictionary<ParameterInfo,string, object>(x => x.Name, x => values[valueIndex++]);
        }

        protected object GetExpressionValue(MemberExpression x)
        {
            var expressionValue = (x.Expression as ConstantExpression).Value;
            var result = x.Member.GetMemberValue(expressionValue);
            return result;
        }

        public RestfulProxy()
        {
            Args = new Dictionary<string, object>();
        }
    }

    public class with_one_arg_expression
    {
        protected static RestfulProxy<ITestService> proxy;

        private Because of = () =>
                                        {
                                            proxy = new RestfulProxy<ITestService>();
                                            var x1 = "test";
                                            proxy.Call(x => x.OneArgCall(x1));
                                        };

        private It should_have_arg_list = () => proxy.Args.Count.ShouldEqual(1);
    }
}
