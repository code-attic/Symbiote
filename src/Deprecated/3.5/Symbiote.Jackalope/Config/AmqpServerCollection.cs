using System.Configuration;

namespace Symbiote.Jackalope.Config
{
    public class AmqpServerCollection : ConfigurationElementCollection
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
            return new AmqpServerConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as AmqpServerConfiguration).Address;
        }
    }
}