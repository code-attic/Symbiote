using System;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope;
using Symbiote.Warren;

namespace WarrenService
{
    public class MessageHandler : IMessageHandler<SocketMessage>
    {
        protected IBus _bus;

        public void Process(SocketMessage message, IResponse response)
        {
            try
            {
                "Client {0} sez: {1}".ToInfo<ClientMessage>(message.From, message.Body);
                response.Acknowledge();

                _bus.Send("client", new SocketMessage() {Body = "If you ever speak to me that way again. I'll kill your face.", To = "client"});
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public MessageHandler(IBus bus)
        {
            _bus = bus;
        }
    }
}