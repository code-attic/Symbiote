using System.Web;
using StructureMap;
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
            ObjectFactory.GetAllInstances<IHttpModule>()
                .ForEach(x => x.Init(this));
        }
    }
}