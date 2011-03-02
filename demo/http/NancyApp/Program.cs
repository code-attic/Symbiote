using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using Nancy;
using Nancy.Routing;
using Nancy.ViewEngines;
using NancyApp.Models;
using Spark.FileSystem;
using Symbiote.Core;
using Symbiote.Http.Owin;
using Symbiote.Nancy;
using Symbiote.StructureMapAdapter;
using Symbiote.Daemon;
using Symbiote.Messaging;
using Symbiote.Http;
using Nancy.ViewEngines.Spark;
using Nancy.ViewEngines.Razor;
using Symbiote.Log4Net;

namespace NancyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Initialize()
                .Daemon(x => x.Arguments(args).Name("Nancy"))
                .HttpHost( x => x
                    .ConfigureHttpListener(h => h.AddPort(8989))
                    .RegisterApplications(h => h
                        .DefineApplication<NancyApplication>(r => true)))
                .AddConsoleLogger<IApplication>(x => x.Info().MessageLayout(m => m.Message().Newline()))
                .RunDaemon();
                
        }
    }

    public class MainModule : NancyModule
    {
        public MainModule(IRouteCacheProvider routeCacheProvider)
        {
            Get["/"] = Home;
        }

        public Response Home(dynamic wtf)
        {
            return View.Razor(@"../../Views/home.cshtml",
                              new FunTimes()
                                  {
                                      What = "OWIN and Nancy",
                                      Description = "Together at friggin' last. Thanks to Symbiote :)"
                                  });
        }
    }

    public static class Extensions
    {
        public static Action<Stream> Razor(this IViewEngine source, string name)
        {
            return source.Razor<object>(name, null);
        }

        public static Action<Stream> Razor<TModel>(this IViewEngine source, string name, TModel model)
        {
            var viewEngine = new MyRazorEngine();

            return stream =>
            {
                var result = viewEngine.RenderView(name, model);
                result.Execute(stream);
            };
        }
    }

    public class MyRazorEngine : ViewEngine
    {
        public MyRazorEngine() : base( Assimilate.GetInstanceOf<IViewLocator>(), new RazorViewCompiler() )
        {
        }
    }

    public class NancyHost : IDaemon
    {
        public IHost HttpHost { get; set; }

        public void Start()
        {
            HttpHost.Start();
        }

        public void Stop()
        {
            HttpHost.Stop();
        }

        public NancyHost(IHost httpHost)
        {
            HttpHost = httpHost;
            var x = Assimilate.GetInstanceOf<MainModule>();
        }
    }
}
