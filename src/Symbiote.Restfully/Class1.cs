using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using System.Web.Hosting;
using StructureMap;
using StructureMap.Pipeline;
using Symbiote.Core;
using Symbiote.Core.Extensions;

namespace Symbiote.Restfully
{
    public static class HttpServerAssimilation
    {
        public static IAssimilate HttpServer(this IAssimilate assimilate, Action<HttpServerConfigurator> configure)
        {
            var configurator = new HttpServerConfigurator(new HttpServerConfiguration());
            configure(configurator);

            assimilate
                .Dependencies(x =>
                                  {
                                      x.For<IHttpServerConfiguration>().Use(configurator.GetConfiguration());
                                      x.For<IHttpServer>().Use<HttpServer>();
                                  });

            return assimilate;
        }
    }

    public enum HttpVerb
    {
        GET,
        PUT,
        POST,
        DELETE,
        LOCK,
        UNLOCK,
        SEARCH,
        COPY,
        MOVE,
    }

    public class ResourceRequest
    {
        protected IHttpServerConfiguration Configuration { get; set; }

        public HttpListenerContext Context { get; protected set; }
        public HttpListenerRequest Request { get { return Context.Request; } }
        public HttpListenerResponse Response { get { return Context.Response; } }
        public string Resource { get; protected set; }
        public string Action { get; protected set; }
        public bool IsValid { get; protected set; }

        protected virtual void Initialize()
        {
            var splits = Context.Request.Url.AbsolutePath.Split('/');

            if (splits.Length == 2)
            {
                Resource = splits[0];
                Action = splits[1];
                IsValid = true;
            }
            else
            {
                IsValid = false;
                return;
            }
        }

        public ResourceRequest(HttpListenerContext context, IHttpServerConfiguration configuration)
        {
            Context = context;
            Configuration = configuration;
            Initialize();
        }
    }

    public interface IHttpServerConfiguration
    {
        IList<string> HostedUrls { get; set; }
        int Port { get; set; }
        AuthenticationSchemes AuthSchemes { get; set; }
        string DefaultService { get; set; }
        string DefaultAction { get; set; }

        void UseDefaults();
    }

    public class HttpServerConfiguration : IHttpServerConfiguration
    {
        public AuthenticationSchemes AuthSchemes { get; set; }
        public IList<string> HostedUrls { get; set; }
        public int Port { get; set; }
        public string DefaultService { get; set; }
        public string DefaultAction { get; set; }

        public void UseDefaults()
        {
            //set defaults
            Port = 8420;
            AuthSchemes = AuthenticationSchemes.Anonymous;
            HostedUrls.Add(@"http://localhost:8420/");
        }

        public HttpServerConfiguration()
        {
            HostedUrls = new List<string>();
        }
    }

    public class HttpServerConfigurator
    {
        private IHttpServerConfiguration _configuration;

        public HttpServerConfigurator Port(int port)
        {
            _configuration.Port = port;
            return this;
        }

        public HttpServerConfigurator UseDefaults()
        {
            _configuration.UseDefaults();
            return this;
        }

        public IHttpServerConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public HttpServerConfigurator(IHttpServerConfiguration configuration)
        {
            _configuration = configuration;
        }
    }

    public interface IHttpServer : IDisposable
    {
        void Start();
        void Stop();
    }

    public class HttpServer : IHttpServer
    {
        protected IHttpServerConfiguration _configuration;
        protected HttpListener _listener;

        public void Start()
        {
            CreateListener();
            BeginListening();
        }

        protected virtual void BeginListening()
        {
            _listener.BeginGetContext(ProcessRequest, null);
        }

        protected virtual void ProcessRequest(IAsyncResult result)
        {
            var listenerContext = _listener.EndGetContext(result);
            var httpContext = GetContext(listenerContext);
            
        }

        protected HttpContextBase GetContext(HttpListenerContext context)
        {
            var httpListenerRequest = context.Request;
            var request = new HttpRequest(
                    "",
                    httpListenerRequest.Url.ToString(),
                    httpListenerRequest.QueryString.ToString()
                );
            var response = new HttpResponse(new StringWriter());
            var httpContext = new HttpContext(request, response);
            return new HttpContextWrapper(httpContext);
        }

        protected virtual void CreateListener()
        {
            _listener = new HttpListener();
            _configuration
                .HostedUrls
                .ForEach(x => _listener.Prefixes.Add(x));

            _listener.AuthenticationSchemes = _configuration.AuthSchemes;
            _listener.Start();
        }

        public void Stop()
        {
            _listener.Abort();
            _listener.Stop();
        }

        public HttpServer(IHttpServerConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Dispose()
        {
            
        }
    }

    public class ServiceRoute<THandler> : Route, IServiceRoute
        where THandler : IHttpHandler
    {
        public IHttpHandler GetHandler(RequestContext requestContext)
        {
            return ObjectFactory.GetInstance<THandler>(new ExplicitArguments(new Dictionary<string, object>() { {"requestContext", requestContext} }));
        }

        public ServiceRoute(string url, IRouteHandler routeHandler) : base(url, routeHandler)
        {
        }

        public ServiceRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler) : base(url, defaults, routeHandler)
        {
        }

        public ServiceRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler) : base(url, defaults, constraints, routeHandler)
        {
        }

        public ServiceRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler) : base(url, defaults, constraints, dataTokens, routeHandler)
        {
        }
    }

    public class RouteDispatcher : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var serviceRoute = requestContext.RouteData.Route as IServiceRoute;
            return serviceRoute.GetHandler(requestContext);
        }
    }

    public interface IServiceRoute
    {
        IHttpHandler GetHandler(RequestContext requestContext);
    }

    public abstract class HttpRequestHandler : IHttpHandler
    {
        protected IHttpServerConfiguration _configuration;
        protected RequestContext _requestContext;

        public abstract void ProcessRequest(HttpContext context);

        public bool IsReusable
        {
            get { return false; }
        }

        protected HttpRequestHandler(RequestContext requestContext)
        {
            _requestContext = requestContext;
        }
    }
}
