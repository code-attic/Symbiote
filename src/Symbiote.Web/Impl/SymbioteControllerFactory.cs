using System.Web.Mvc;
using System.Web.Routing;
using StructureMap;

namespace Symbiote.Web.Impl
{
    public class SymbioteControllerFactory : DefaultControllerFactory
    {
        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            var controllerType = base.GetControllerType(requestContext, controllerName);
            return ObjectFactory.GetInstance(controllerType) as IController;
        }
    }
}