namespace Demo.Messages
{
    public class Response
    {
        public virtual string Body { get; set; }

        public Response(string body)
        {
            Body = body;
        }
    }
}