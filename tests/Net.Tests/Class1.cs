using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;

namespace Net.Tests
{
    public interface IHttpListener
    {
        bool IsListening { get; }
        HttpListenerPrefixCollection Prefixes { get; }
        string Realm { get; set; }
        int Port { get; }
        
        void Abort();
        IAsyncResult BeginGetContext(AsyncCallback callback, object state);
        void Close();
        IHttpContext EndGetContext(IAsyncResult result);
        void Start();
        void Stop();

    }

    public class HttpListener : IHttpListener
    {
        protected TcpListener Listener { get; set; }
        public bool IsListening { get; private set; }
        public HttpListenerPrefixCollection Prefixes { get; private set; }
        public int Port { get; private set; }
        public string Realm { get; set; }

        public void Abort()
        {
            if(Listener != null)
            {
                
            }
        }

        public IAsyncResult BeginGetContext(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public IHttpContext EndGetContext(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public HttpListener(int port)
        {
            Port = port;
        }
    }

    public interface IHttpRequest
    {
        
        ulong ConnectionId { get; }
        long ContentLength { get; }
        string ContentType { get; }
        Encoding ContentEncoding { get; }
        CookieCollection Cookies { get; }

        WebHeaderCollection Headers { get; }
        IHttpContext HttpContext { get; set; }
        string HttpMethod { get; }
        
        bool IsDisposed { get; }
        bool IsSecureConnection { get; }
        bool KeepAlive { get; }
        IPEndPoint LocalEndPoint { get; }
        Version ProtocolVersion { get; }
        string RawUrl { get; }
        IPEndPoint RemoteEndPoint { get; }
        ulong RequestId { get; }
        
        string UrlHost { get; }
        string UrlPath { get; }
        string UrlQuery { get; }
        
    }

    public class HttpRequest : IHttpRequest
    {
        protected Socket Socket { get; set; }
        public ulong ConnectionId { get; private set; }
        public long ContentLength { get; private set; }
        public string ContentType { get; private set; }
        public Encoding ContentEncoding { get; private set; }
        public CookieCollection Cookies { get; private set; }
        public WebHeaderCollection Headers { get; private set; }
        public IHttpContext HttpContext { get; set; }
        public string HttpMethod { get; private set; }
        public bool IsDisposed { get; private set; }
        public bool IsSecureConnection { get; private set; }
        public bool KeepAlive { get; private set; }
        public IPEndPoint LocalEndPoint { get; private set; }
        public Version ProtocolVersion { get; private set; }
        public string RawUrl { get; private set; }
        public IPEndPoint RemoteEndPoint { get; private set; }
        public ulong RequestId { get; private set; }
        public string UrlHost { get; private set; }
        public string UrlPath { get; private set; }
        public string UrlQuery { get; private set; }

        public HttpRequest(Socket socket)
        {
            Socket = socket;
        }

        protected void Process()
        {
            
        }
    }

    public interface IHttpResponse
    {
        Encoding ContentEncoding { get; set; }
        long ContentLength { get; set; }
        string ContentType { get; set; }
        CookieCollection Cookies { get; set; }
        WebHeaderCollection Headers { get; set; }
        bool KeepAlive { get; set; }
        Stream OutputStream { get; set; }
        Version ProtocolVersion { get; set; }
        string RedirectLocation { get; set; }
        bool SendChunked { get; set; }
        int StatusCode { get; set; }
        string StatusDescription { get; set; }


        void Abort();
        void AddHeader(string name, string value);
        void AppendCookie(Cookie cookie);
        void AppendHeader(string name, string value);
        void Close();
        void Close(byte[] responseEntity, bool willBlock);


        void Redirect(string url);
        void SetCookie(Cookie cookie);
    }

    public interface IHttpContext
    {
        IHttpRequest Request { get; set; }
        IHttpResponse Response { get; set; }
        IPrincipal User { get; set; }
    }

    public class HttpContext : IHttpContext
    {
        protected Socket Socket { get; set; }
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }
        public IPrincipal User { get; set; }

        public HttpContext(Socket socket)
        {
            Socket = socket;
            Request = new HttpRequest(socket);
            Response = new HttpResponse(socket);
        }
    }
}
