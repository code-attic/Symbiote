using System;
using System.IO.Pipes;
using System.Security.Principal;

namespace Symbiote.Messaging.Impl.Channels
{
    public class NamedPipeChannelDefinition : BaseChannelDefinition
    {
        public string Server { get; set; }
        public PipeDirection Direction { get; set; }
        public PipeAccessRights Rights { get; set; }
        public PipeTransmissionMode Mode { get; set; }
        public PipeOptions Options { get; set; }
        public TokenImpersonationLevel Impersonation { get; set; }
        public int ConnectionTimeout { get; set; }
        public override Type ChannelType { get { return typeof(NamedPipeChannel); } }
        public override Type FactoryType { get { return typeof(NamedPipeChannelFactory); } }

        public NamedPipeChannelDefinition(string name)
        {
            Name = name;
            Server = "Symbiote.Pipes.Host";
            Direction = PipeDirection.InOut;
            Rights = PipeAccessRights.FullControl;
            Mode = PipeTransmissionMode.Message;
            Options = PipeOptions.Asynchronous;
            Impersonation = TokenImpersonationLevel.Anonymous;
            ConnectionTimeout = 1000;
        }
    }
}
