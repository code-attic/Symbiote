using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Symbiote.Eidetic.Config
{
    public class MemcachedServerList : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MemcachedServer();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as MemcachedServer).Address;
        }
    }
}
