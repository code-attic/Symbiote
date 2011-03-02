using System;

namespace Symbiote.Jackalope
{
    public class NoRouteDefinedException : Exception
    {
        public NoRouteDefinedException(string message) : base(message)
        {   
        }
    }
}