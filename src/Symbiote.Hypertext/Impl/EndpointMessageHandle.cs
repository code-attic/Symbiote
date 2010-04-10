using Symbiote.Jackalope;
using Symbiote.Jackalope.Impl;

namespace Symbiote.Telepathy
{
    public class EndpointMessageHandle : IMessageHandler<IAmqpEndpointConfiguration>
    {
        private IEndpointIndex _endpointIndex;

        public void Process(IAmqpEndpointConfiguration message, IMessageDelivery messageDelivery)
        {
            var wrapper = BusEndPoint.CreateFromAmqpEndpoint(message);
            _endpointIndex.AddEndpoint(wrapper);
        }

        public EndpointMessageHandle(IEndpointIndex endpointIndex)
        {
            _endpointIndex = endpointIndex;
        }
    }
}