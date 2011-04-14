using System;
using Symbiote.Http.Config;
using Symbiote.Http.Owin;
using Symbiote.Http.Owin.Impl;
using Symbiote.Net;
using System.Linq;

namespace Symbiote.Http.NetAdapter.Socket
{
    public class SocketApplicationProxy
		: IDisposable
    {
        public IRouteRequest Router { get; set; }
        public ISocket Connection { get; set; }
        public Request RequestContext { get; set; }
        public RequestReader Reader { get; set; }
        public ResponseHelper Response { get; set; }
        public ResponseWriter Writer { get; set; }
        public HttpWebConfiguration WebConfiguration { get; set; }
		public bool Disposed { get; protected set; }

        public void Close()
        {
            Connection = null;
        }

        private void OnRequest( ArraySegment<byte> segment )
        {
            var bytes = new byte[ segment.Count ];
            if ( segment.Count > 0 )
            {
                Buffer.BlockCopy( segment.Array, segment.Offset, bytes, 0, segment.Count );
                RequestParser.PopulateRequest( RequestContext, segment );
                if( RequestContext.Valid )
                {
                    var application = Router.GetApplicationFor( RequestContext );
                    application.Process( RequestContext, Response, ApplicationException );

                    if( !RequestContext.CanHaveBody && RequestContext.HeadersComplete )
                    {
                        application.OnComplete();
                    }
                    else
                    {   
                        Reader.ReadNext();
                    }
                }
            }
        }

        public void Start( ISocketAdapter connection, Action<ISocketAdapter> onClose )
        {
            Response = new ResponseHelper( WebConfiguration );
            Reader = new RequestReader( connection, Close );
            Writer = new ResponseWriter( connection, Close );
            Response.Setup( Writer );
            RequestContext.Body = Reader.Setup;
            RequestContext.OnBody = Reader.CacheBytes;
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
            RequestContext = new Request();
        }
		
        public void Dispose ()
		{
			Close();
		}

		~SocketApplicationProxy()
		{
			if( !Disposed )
				Dispose();
		}
    }
}