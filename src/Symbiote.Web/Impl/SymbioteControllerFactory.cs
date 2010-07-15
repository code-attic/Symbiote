using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.ServiceLocation;

namespace Symbiote.Web.Impl
{
    public class SymbioteControllerFactory : DefaultControllerFactory
    {
        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            var controllerType = base.GetControllerType(requestContext, controllerName);
            if(controllerType != null)
                return ServiceLocator.Current.GetInstance(controllerType) as IController;
            return null;
        }
    }
}