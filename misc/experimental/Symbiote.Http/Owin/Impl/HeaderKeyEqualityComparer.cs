using System;
using System.Collections.Generic;

namespace Symbiote.Http.Owin.Impl
{
    public class HeaderKeyEqualityComparer
        : IEqualityComparer<string>
    {
        public bool Equals( string x, string y )
        {
            return string.Equals( x, y, StringComparison.CurrentCultureIgnoreCase );
        }

        public int GetHashCode( string obj )
        {
            return obj.GetHashCode();
        }
    }
}