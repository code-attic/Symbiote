using System.Text;

namespace Messaging.Tests.Pipes
{
    public class BytesToString
        : IPipe<byte[], string>
    {
        public string Process( byte[] input )
        {
            return Encoding.UTF8.GetString( input );
        }
    }
}