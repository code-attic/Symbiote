using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace Symbiote.Net
{
    public class HttpServer : 
        IHttpServer
    {
        protected TcpListener Listener { get; set; }
        protected IHttpServerConfiguration Configuration { get; set; }
        protected bool Running { get; set; }
        protected IPAddress ServerAddress { get; set; }
        protected IPEndPoint ServerEndpoint { get; set; }
        protected ClientFactory ClientFactory { get; set; }
        protected ConcurrentBag<IObserver<HttpContext>> Watchers { get; set; }
        protected IHttpAuthChallenger AuthenticationChallenger { get; set; }

        public void Start()
        {
            Listener.Start();
            Running = true;
            Listen();
        }

        private void Listen()
        {
            if(Running)
                Listener.BeginAcceptTcpClient(ProcessNewClient, null);
        }

        protected void AuthenticationFinished(bool success, HttpContext context)
        {
            Watchers.AsParallel().ForAll(x =>
                                             {
                                                 if(success)
                                                 {
                                                     x.OnNext(context);
                                                 }
                                                 else
                                                 {
                                                     x.OnError(new HttpServerException("Exception occurred authenticating client"));
                                                 }
                                             });
        }

        private void ProcessNewClient(IAsyncResult ar)
        {
            Listen();
            var tcpClient = Listener.EndAcceptTcpClient(ar);
            var httpClient = ClientFactory.GetClient(tcpClient);
            AuthenticationChallenger.ChallengeClient(httpClient, AuthenticationFinished);
        }

        public void Stop()
        {
            Listener.Stop();
            IObserver<HttpContext> watcher = null;
            while(Watchers.TryTake(out watcher))
            {
                watcher.OnCompleted();
            }
        }

        public IDisposable Subscribe(IObserver<HttpContext> observer)
        {
            Watchers.Add(observer);
            return observer as IDisposable;
        }

        protected void InitializeListener()
        {
            ServerAddress = IPAddress.Any;
            ServerEndpoint = new IPEndPoint(ServerAddress, Configuration.Port);
            Listener = new TcpListener(ServerEndpoint);
            Listener.ExclusiveAddressUse = true;
        }

        public HttpServer(IHttpServerConfiguration configuration, IHttpAuthChallenger authChallenger)
        {
            Configuration = configuration;
            InitializeListener();
            ClientFactory = new ClientFactory(configuration);
            AuthenticationChallenger = authChallenger;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}