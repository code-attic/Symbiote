using System;

namespace Symbiote.Jackalope.Impl.Routes
{
    public interface IDefineRoute<TMessage>
    {
        IDefineRoute<TMessage> IncludeChildMessageTypes();
        IDefineRoute<TMessage> SendTo(string exchangeName);
        IDefineRoute<TMessage> RouteBy(Func<TMessage, string> routeMethod);
        IDefineRoute<TMessage> WithRoutingKey(Func<TMessage, string> subjectBuilder);
        IDefineRoute<TMessage> WithRoutingKey(string subject);
    }
}