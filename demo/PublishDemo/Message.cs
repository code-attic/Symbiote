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

    public class Response
    {
        public virtual string Body { get; set; }

        public Response(string body)
        {
            Body = body;
        }
    }
}