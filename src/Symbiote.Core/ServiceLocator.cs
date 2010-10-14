using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;

namespace Symbiote.Core
{
    public class ServiceLocator
    {
        public static Microsoft.Practices.ServiceLocation.IServiceLocator Current
        {
            get { return Microsoft.Practices.ServiceLocation.ServiceLocator.Current; }
        }

        public static void SetLocatorProvider(IServiceLocator provider)
        {
            Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(() => provider);
        }
    }
}
