using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Eidetic.Extensions
{
    public interface IRemembrance
    {
        IRemembrance Is<T>(T value);
        IRemembrance For(TimeSpan time);
        IRemembrance Until(DateTime expiration);
    }
}
