using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using jabber;
using jabber.client;
using jabber.protocol.client;
using MbUnit.Framework;
using Symbiote.Core;
using Symbiote.TestSpec;
using Symbiote.Jackalope;
using StructureMap;

namespace Symbiote.Jabber.Tests
{
    [TestFixture]
    public class WhenConnectingToServer : Spec
    {

        public JabberClient client;
        public IBus messager;
        public bool registered = false;

        public override void Initialize()
        {
            
        }

        void client_OnRegistered(object sender, jabber.protocol.client.IQ iq)
        {
            registered = true;
        }

        bool client_OnInvalidCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        void client_OnStreamError(object sender, System.Xml.XmlElement rp)
        {
            client.Dispose();
            
            throw new Exception(rp.Value);
        }

        void client_OnError(object sender, Exception ex)
        {
            client.Dispose();
            throw ex;
        }

        void client_OnWriteText(object sender, string txt)
        {
            
        }

        void client_OnReadText(object sender, string txt)
        {
            Console.WriteLine("From {0}: {1}", sender, txt);
        }

        void client_OnAuthError(object sender, System.Xml.XmlElement rp)
        {
            throw new Exception("Uhhhhh crap");
        }

        public override void Arrange()
        {
            
        }

        public override void Act()
        {
            
        }

        [Assert]
        public void ShouldSendMessageToXMPP()
        {
            Assimilate.Core().Jackalope(x => x.AddServer(s => s.Address("localhost").AMQP08()));

            messager = ObjectFactory.GetInstance<IBus>();
            messager.AddEndPoint(x => x.QueueName("alex").Exchange("rabbit", ExchangeType.direct));
            messager.Send("alex", "Hello");
            //messager.Subscribe<Handler, string>("alex", null);
        }
        
        [Assert]
        public void ShouldJabber()
        {
            base.Initialize();
            client = new JabberClient();
            var jid = new JID("sliver", "ALEXR-NB2.sommet.local", "");
            client.User = jid.User;
            client.Server = jid.Server;
            client.Port = 5222;
            client.NetworkHost = "127.0.0.1";
            client.Resource = "Jabber.Net Symbiote";
            client.Password = "sliver";
            client.AutoStartTLS = true;
            client.AutoPresence = true;
            client.OnAuthError += new jabber.protocol.ProtocolHandler(client_OnAuthError);
            client.OnReadText += new bedrock.TextHandler(client_OnReadText);
            client.OnWriteText += new bedrock.TextHandler(client_OnWriteText);
            client.OnError += new bedrock.ExceptionHandler(client_OnError);
            client.OnStreamError += new jabber.protocol.ProtocolHandler(client_OnStreamError);
            client.OnInvalidCertificate += new System.Net.Security.RemoteCertificateValidationCallback(client_OnInvalidCertificate);

            client.AutoLogin = true;
            client.AutoReconnect = 3f;
            client.AutoPresence = true;

            client.Connect();

            client.OnRegistered += new IQHandler(client_OnRegistered);

            Thread.Sleep(5000);
            try
            {
                var alex = new JID("alex@alexr-nb2.sommet.local");
                client.Subscribe(alex, "alex", new string[] { });

                var msg = new Message(client.Document);
                msg.Body = "hi!";
                msg.To = "alex@alexr-nb2.sommet.local";
                client.Write(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public override void Finish()
        {
            base.Finish();
            messager.Dispose();
            client.Dispose();
        }
    }

    public class Handler : IMessageHandler<string>
    {
        private IBus _bus;

        public Handler(IBus bus)
        {
            _bus = bus;
        }

        public void Process(string message, IResponse response)
        {
            _bus.Send("alex", message);
        }
    }
}
