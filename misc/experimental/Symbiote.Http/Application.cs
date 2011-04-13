using System;
using Symbiote.Http.Owin;

namespace Symbiote.Http
{
    public abstract class Application
        : IApplication
    {
        public Action Cancel { get; set; }
        public IRequest Request { get; set; }
        public IBuildResponse Response { get; set; }
        public Action<Exception> OnApplicationException { get; set; }
		
        public virtual void Process( IRequest request, IBuildResponse response, Action<Exception> onException )
        {
            Request = request;
            Cancel = Request.Body( OnNext, OnError, OnComplete );
            Response = response;
            OnApplicationException = onException;
        }

        public bool OnNext( ArraySegment<byte> data, Action continuation )
		{
			return HandleRequestSegment( data, continuation );
		}
		
		public virtual bool HandleRequestSegment( ArraySegment<byte> data, Action continuation )
		{
			return false;
		}

        public abstract void OnError( Exception exception );

        public void OnComplete()
		{
			CompleteResponse();			
		}
		
		public void Done()
		{
			Cancel = null;
			Response.Dispose();
			Request.Dispose();
		}
		
		public abstract void CompleteResponse();
    }
}