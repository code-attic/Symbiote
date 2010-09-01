using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Microsoft.Practices.ServiceLocation;

namespace Symbiote.Caliburn.Micro
{
    public class SymbioteBootStrapper<RootModel> : Bootstrapper<RootModel>
    {
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return ServiceLocator.Current.GetAllInstances(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            object instance;
            if(string.IsNullOrEmpty(key))
            {
                instance = ServiceLocator.Current.GetInstance(service);
            }
            else
            {
                instance = ServiceLocator.Current.GetInstance(service, key);
            }
            instance = instance ?? base.GetInstance(service, key);
            return instance;
        }
    }
}
