using System;

namespace Symbiote.Redis
{
    public class ResponseException : Exception
    {
        public ResponseException(string code)
            : base("Response error")
        {
            Code = code;
        }

        public string Code { get; private set; }
    }
}