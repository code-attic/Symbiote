using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Core.Utility
{
    public class Future
    {
        public static FutureResult<T> Of<T>( Func<T> call )
        {
            return new FutureResult<T>( call );
        }

        public static FutureCallback<T> Of<T>(Action<Action<T>> call)
        {
           return new FutureCallback<T>( call );
        }
    }
}
