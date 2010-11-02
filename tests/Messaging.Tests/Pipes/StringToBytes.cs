using System.Text;

namespace Messaging.Tests.Pipes
{
    public class StringToBytes
        : IPipe<string, byte[]>
    {
        public byte[] Process( string input )
        {
            return Encoding.UTF8.GetBytes( input );
        }
    }
}