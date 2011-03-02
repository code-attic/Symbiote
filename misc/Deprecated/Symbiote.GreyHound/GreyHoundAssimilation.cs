using System;
using System.Linq;
using System.Text;
using MassTransit;
using StructureMap;
using Symbiote.Core;

namespace Symbiote.GreyHound
{
    public static class GreyHoundAssimilation
    {
        public static IAssimilate GreyHound(this IAssimilate assimilate, Action<GreyHoundConfigurator> configure)
        {
            var registry = GetRegistry(configure);
            WireUp(registry);
            AutoSubscribe();
            return assimilate;
        }

        private static void WireUp(GreyHoundRegistry registry)
        {
            ObjectFactory
                .Configure(x =>
                               {
                                   x.AddRegistry(registry);
                                   x.Scan(s =>
                                              {
                                                  s.TheCallingAssembly();
                                                  s.AddAllTypesOf(typeof (Consumes<>.All));
                                              });
                               });
        }

        private static GreyHoundRegistry GetRegistry(Action<GreyHoundConfigurator> configure)
        {
            var configurator = new GreyHoundConfigurator();
            configure(configurator);
            return new GreyHoundRegistry(configurator);
        }

        private static void AutoSubscribe()
        {
            using(var bus = ObjectFactory.GetInstance<IServiceBus>())
            {
                ObjectFactory
                    .Container
                    .Model
                    .PluginTypes
                    .Where(x => typeof (Consumes<>.All).IsAssignableFrom(x.PluginType))
                    .SelectMany(x => x.Instances)
                    .Each(x => bus.Subscribe(x.ConcreteType));
            }
        }
    }
}
