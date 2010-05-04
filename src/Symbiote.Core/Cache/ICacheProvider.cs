using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Core.Cache
{
    public interface ICacheProvider
    {
        void Store<T>(string key, T value);
        T Get<T>(string key);
        void Remove(string key);
    }
}
