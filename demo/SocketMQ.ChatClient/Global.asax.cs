using System.Web.Mvc;
using System.Web.Routing;
using Symbiote.Core;
using Symbiote.Web;
using Spark.Web.Mvc;

namespace SocketMQ.ChatClient
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : SymbioteMvcApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            Assimilate
                .Core()
                .Web(x => x
                        .SupplyControllerDependencies()
                        .UseSparkViews());
        }
    }
}