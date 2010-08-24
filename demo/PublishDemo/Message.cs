using System;

namespace PublishDemo
{
    public class Message
    {
        public virtual string Body { get; set; }
        public virtual DateTime Created { get; protected set; }

        public Message(string body)
        {
            Body = body;
            Created = DateTime.Now;
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