using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

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

        public void Start()
        {
            Listener.Start();
            Running = true;

            while(Running)
            {
                Listener.BeginAcceptTcpClient(ProcessNewClient, null);
            }
        }

        private void ProcessNewClient(IAsyncResult ar)
        {
            var tcpClient = Listener.EndAcceptTcpClient(ar);
            var httpClient = ClientFactory.GetClient(tcpClient);

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
            ServerEndpoint = new IPEndPoint(ServerAddress, 8001);
            Listener = new TcpListener(ServerEndpoint);
            Listener.ExclusiveAddressUse = true;
        }

        public HttpServer(IHttpServerConfiguration configuration)
        {
            Configuration = configuration;
            InitializeListener();
            ClientFactory = new ClientFactory(configuration);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}