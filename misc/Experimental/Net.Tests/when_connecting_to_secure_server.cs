using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Machine.Specifications;

namespace Net.Tests
{
    public class when_connecting_to_secure_server : with_tcp_client
    {
        protected static string message = "";

        private Because of = () =>
                                 {
                                     using (var stream = new SslStream(
                                         client.GetStream(),
                                         false,
                                         validateCert2))
                                     {

                                         stream.AuthenticateAsClient("symbiote");
                                         using (var reader = new StreamReader(stream))
                                         {
                                             message = reader.ReadLine();
                                         }

                                         stream.Close();
                                         client.Close();
                                     }
                                 };

        protected static bool validateCert2(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }
        
        private It should_have_message = () => message.ShouldEqual("HOWDY!");
    }
}