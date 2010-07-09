using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Jackalope.Impl.Routes
{
    public interface IDefineRoute
    {
        IDefineRoute For<TMessage>();
        IDefineRoute SendTo(string exchangeName);
        IDefineRoute WithSubject<TMessage>(Func<TMessage, string> subjectBuilder);
        IDefineRoute WithSubject(string subject);
    }

    public class RouteBuilder : IDefineRoute
    {
        public IDefineRoute For<TMessage>()
        {

            return this;
        }

        public IDefineRoute SendTo(string exchangeName)
        {

            return this;
        }

        public IDefineRoute WithSubject<TMessage>(Func<TMessage, string> subjectBuilder)
        {

            return this;
        }

        public IDefineRoute WithSubject(string subject)
        {

            return this;
        }
    }

    public interface IRouteMessage<TMessage>
    {
        string ExchangeName { get; set; }
        Func<TMessage, string> SubjectBuilder { get; set; }
    }

    public class MessageRoute<TMessage> : 
        IRouteMessage<TMessage>
    {
        public string ExchangeName { get; set; }
        public Func<TMessage, string> SubjectBuilder { get; set; }
    }
}
