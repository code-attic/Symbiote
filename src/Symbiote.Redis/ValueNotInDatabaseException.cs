using System;

namespace Symbiote.Redis
{
    public class ValueNotInDatabaseException : Exception
    {
        public string Code { get; private set; }

        public ValueNotInDatabaseException(string code)
            : base("Value not in database error")
        {
            Code = code;
        }
    }
}
