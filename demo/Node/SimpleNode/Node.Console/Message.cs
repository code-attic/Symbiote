namespace Node.Console
{
    public class Message
    {
        public string Text { get; set; }

        public Message() {}

        public Message( string text )
        {
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}