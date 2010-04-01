namespace PublishDemo
{
    public class Message
    {
        public virtual string Body { get; set; }

        public Message(string body)
        {
            Body = body;
        }
    }
}