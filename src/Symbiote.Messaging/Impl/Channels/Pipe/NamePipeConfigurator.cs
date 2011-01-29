using System.IO.Pipes;
using System.Security.Principal;

namespace Symbiote.Messaging.Impl.Channels
{
    public class NamePipeConfigurator
    {
        public NamedPipeChannelDefinition Defintion { get; set; }

        public NamePipeConfigurator Server(string server)
        {
            Defintion.Server = server;
            return this;
        }

        public NamePipeConfigurator Direction(PipeDirection direction)
        {
            Defintion.Direction = direction;
            return this;
        }

        public NamePipeConfigurator Rights(PipeAccessRights rights)
        {
            Defintion.Rights = rights;
            return this;
        }

        public NamePipeConfigurator Mode(PipeTransmissionMode mode)
        {
            Defintion.Mode = mode;
            return this;
        }

        public NamePipeConfigurator Options(PipeOptions options)
        {
            Defintion.Options = options;
            return this;
        }

        public NamePipeConfigurator Impersonation(TokenImpersonationLevel impersonationLevel)
        {
            Defintion.Impersonation = impersonationLevel;
            return this;
        }

        public NamePipeConfigurator ConnectTimeout(int msTimeout)
        {
            Defintion.ConnectionTimeout = msTimeout;
            return this;
        }

        public NamePipeConfigurator( string channelName )
        {
            Defintion = new NamedPipeChannelDefinition( channelName );
        }
    }
}