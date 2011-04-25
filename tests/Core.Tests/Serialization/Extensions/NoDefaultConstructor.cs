namespace Core.Tests.Serialization.Extensions
{
    public class NoDefaultConstructor
    {
        public string Message { get; set; }

        public NoDefaultConstructor( string message )
        {
            Message = message;
        }
    }
}