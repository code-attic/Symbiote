using System;

namespace DaemonDemo
{
    public interface IMessage
    {
        string Body { get; set; }
        DateTime Created { get; set; }
        bool Rejection { get; set; }
        bool Reject { get; set; }
    }

    [Serializable]
    public class MessageOne : IMessage
    {
        public virtual string Body { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual bool Rejection { get; set; }
        public virtual bool Reject { get; set; }

        public static MessageOne Create(string message)
        {
            return new MessageOne()
                   {
                       Body = message,
                       Created = DateTime.Now
                   };
        }
    }

    [Serializable]
    public class MessageTwo : IMessage
    {
        public virtual string Body { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual bool Rejection { get; set; }
        public virtual bool Reject { get; set; }

        public static MessageTwo Create(string message)
        {
            return new MessageTwo()
            {
                Body = message,
                Created = DateTime.Now
            };
        }
    }
}