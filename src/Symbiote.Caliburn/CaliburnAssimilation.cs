using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.Core;
using Caliburn.StructureMap;
using StructureMap;
using Caliburn.PresentationFramework;
using Symbiote.Core;

namespace Symbiote.Caliburn
{
    public static class CaliburnAssimilation
    {
        private static CoreConfiguration caliburnHook { get; set; }

        public static IAssimilate Caliburn(this IAssimilate assimilate)
        {
            UseDefaults();
            return assimilate;
        }

        public static IAssimilate Caliburn(this IAssimilate assimilate, Action<IConfigurationHook> configurator)
        {
            configurator(caliburnHook);
            return assimilate;
        }

        public static void StartApplication(this IAssimilate assimilate)
        {
            caliburnHook.Start();
        }

        private static void UseDefaults()
        {
            var adapter = new StructureMapAdapter(ObjectFactory.Container);
            caliburnHook = (CaliburnFramework
                            .ConfigureCore(adapter)
                            .WithPresentationFramework() as IConfigurationHook).Core;
        }
    }
}
