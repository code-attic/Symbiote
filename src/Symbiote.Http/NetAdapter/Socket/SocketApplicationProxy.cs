using System;
using Symbiote.Http.Config;
using Symbiote.Http.Owin;
using Symbiote.Http.Owin.Impl;
using Symbiote.Net;

namespace Symbiote.Http.NetAdapter.Socket
{
    public class SocketApplicationProxy
    {
        public IRouteRequest Router { get; set; }
        public Action<ISocketAdapter> OnClose { get; set; }
        public ISocketAdapter Connection { get; set; }
        public Request ProxyRequest { get; set; }
        public RequestReader Reader { get; set; }
        public ResponseHelper Response { get; set; }
        public ResponseWriter Writer { get; set; }
        public HttpWebConfiguration WebConfiguration { get; set; }

        public void Close()
        {
            OnClose(Connection);
            Connection = null;
        }

        private void OnRequest( ArraySegment<byte> segment )
        {
            var bytes = new byte[ segment.Count ];
            Buffer.BlockCopy( segment.Array, segment.Offset, bytes, 0, segment.Count );
            RequestParser.PopulateRequest( ProxyRequest, segment, Reader.CacheBytes );
            ProxyRequest.Body = Reader.Setup;
            var application = Router.GetApplicationFor( ProxyRequest );
            application.Process( ProxyRequest, Response, ApplicationException );
            Reader.ReadNext();
        }

        public void Start( ISocketAdapter connection, Action<ISocketAdapter> onClose )
        {
            Response = new ResponseHelper( WebConfiguration );
            Reader = new RequestReader( connection, Close );
            Writer = new ResponseWriter( connection, Close );
            Response.Setup( Writer );
            OnClose = onClose;
            Connection = connection;
            Connection.Read( OnRequest );
        }

        public void ApplicationException( Exception exception )
        {

        }

        public SocketApplicationProxy( IRouteRequest router, HttpWebConfiguration webConfiguration )
        {
            WebConfiguration = webConfiguration;
            Router = router;
            ProxyRequest = new Request();
        }
    }
}