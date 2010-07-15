using System.Web;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core.Extensions;
using Enumerable = System.Linq.Enumerable;

namespace Symbiote.Web
{
    public class SymbioteMvcApplication : HttpApplication
    {
        public override void Init()
        {
            base.Init();
            InitModules();
        }

        private void InitModules()
        {
            ServiceLocator
                .Current
                .GetAllInstances<IHttpModule>()
                .ForEach(x => x.Init(this));
        }
    }
}