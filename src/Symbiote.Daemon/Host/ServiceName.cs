using Symbiote.Core.Extensions;

namespace Symbiote.Daemon.Host
{
    public class ServiceName
    {
        protected const string NAME_FORMAT = "{0}${1}";
        public string InstanceName { get; set; }
        public string Name { get; set; }
        public string FullName
        {
            get { return NAME_FORMAT.AsFormat(Name, InstanceName); }
        }
    }
}