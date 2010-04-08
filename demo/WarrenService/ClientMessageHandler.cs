using System;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope;
using Symbiote.Warren;

namespace WarrenService
{
    //public class ClientMessageHandler : IMessageHandler<ClientSocketMessage>
    //{
    //    protected IBus _bus;

    //    public void Process(ClientSocketMessage message, IResponse response)
    //    {
    //        try
    //        {
    //            "Client {0} sez: {1}".ToInfo<ClientMessage>(message.From, message.Body);
    //            response.Acknowledge();

    //            _bus.Send(
    //                "client", 
    //                new ServerSocketMessage()
    //                    {
    //                        Body = message.Body, 
    //                        To = message.To,
    //                        From = message.From,
    //                        RoutingKey = message.RoutingKey
    //                    });
    //        }
    //        catch (Exception ex)
    //        {
                
    //            throw;
    //        }
    //    }

    //    public ClientMessageHandler(IBus bus)
    //    {
    //        _bus = bus;
    //    }
    //}
}