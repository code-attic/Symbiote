using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Messaging.Extensions;

namespace Symbiote.Messaging.Impl.Dispatch
{
    public class ResponseDispatcher<TResponse>
        : IDispatchMessage<TResponse>
    {
        protected Action<TResponse> Handle { get; set; }

        public Type ActorType
        {
            get { return null; }
        }
        public bool CanHandle( object payload )
        {
            return payload.IsOfType<TResponse>();
        }

        public bool DispatchForActor
        {
            get { return false; }
        }

        public IEnumerable<Type> Handles
        {
            get { yield return typeof(TResponse); }
        }

        public void Dispatch( IEnvelope envelope )
        {
            var typedEnvelope = envelope as IEnvelope<TResponse>;
            Handle(typedEnvelope.Message);
        }

        public ResponseDispatcher( Action<TResponse> handle )
        {
            Handle = handle;
        }
    }
}
