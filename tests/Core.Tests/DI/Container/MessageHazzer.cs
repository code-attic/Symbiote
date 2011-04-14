using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Core.Tests.DI.Container
{
    public class MessageHazzer : IHazzaMessage
    {
        public string GetMessage()
        {
            return "This is a message from MessageHazzer. Hi!";
        }
    }
}
