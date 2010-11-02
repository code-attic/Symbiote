namespace Messaging.Tests.MessageSerializers
{
    public class IncompatibleMessage
    {
        public string Text { get; set; }

        public IncompatibleMessage( string text )
        {
            Text = text;
        }
    }
}