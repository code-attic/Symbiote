using System;
using RabbitMQ.Client.Events;

namespace Symbiote.Jackalope.Impl
{
    public class Message<T> : IMessage<T> where T : class
    {
        private IChannelProxy _proxy;
        private IChannelProxyFactory _factory;
        private T _body;
        private BasicDeliverEventArgs _args;

        public T MessageBody
        {
            get { return _body; }
        }

        public void Acknowledge()
        {
            Respond(x => x.Acknowledge(_args.DeliveryTag, false), _args.Exchange);
        }

        public void Reject()
        {
            Respond(x => x.Reject(_args.DeliveryTag, true), _args.Exchange);
        }

        public void Reply<TReply>(TReply reply)
            where TReply : class
        {
            if(!_args.BasicProperties.IsReplyToPresent())
                return;

            Respond(x => x.Reply(_args.BasicProperties.ReplyToAddress, _args.BasicProperties, reply), _args.Exchange);
        }

        private void Respond(Action<IChannelProxy> response, string exchange)
        {
            //using(var proxy = _factory.GetProxyForExchange(exchange))
            //{
            //    response(proxy);
            //}
            response(_proxy);
        }

        public Message(IChannelProxy proxy, T body, BasicDeliverEventArgs args)
        {
            //_factory = factory;
            _proxy = proxy;
            _body = body;
            _args = args;
        }
    }
}