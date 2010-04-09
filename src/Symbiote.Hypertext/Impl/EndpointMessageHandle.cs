using Symbiote.Jackalope;
using Symbiote.Jackalope.Impl;

namespace Symbiote.Telepathy
{
    public class EndpointMessageHandle : IMessageHandler<IAmqpEndpointConfiguration>
    {
        private IEndpointManager _endpointManager;

        public void Process(IAmqpEndpointConfiguration message, IRespond respond)
        {
            var wrapper = BusEndPoint.CreateFromAmqpEndpoint(message);
            _endpointManager.AddEndpoint(wrapper);
        }

        public EndpointMessageHandle(IEndpointManager endpointManager)
        {
            _endpointManager = endpointManager;
        }
    }
}