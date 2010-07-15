using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;

namespace Symbiote.Eidetic.Impl
{
    internal class MemoryMananger
    {
        private static IRemember _rememory;

        public static IRemember Memory
        {
            get
            {
                _rememory = _rememory ??
                            ServiceLocator.Current.GetInstance<IRemember>();
                return _rememory;
            }
        }
    }
}
