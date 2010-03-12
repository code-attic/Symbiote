using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace Symbiote.Core.Config
{
    public class StructureMapServiceLocator : IServiceLocator
    {
        public object GetInstance(Type serviceType)
        {
            return ObjectFactory.GetInstance(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return ObjectFactory.GetNamedInstance(serviceType, key);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return ObjectFactory.GetAllInstances(serviceType).Cast<object>();
        }

        public TService GetInstance<TService>()
        {
            return ObjectFactory.GetInstance<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            return ObjectFactory.GetNamedInstance<TService>(key);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return ObjectFactory.GetAllInstances<TService>();
        }

        public object GetService(Type serviceType)
        {
            return GetInstance(serviceType);
        }
    }
}
