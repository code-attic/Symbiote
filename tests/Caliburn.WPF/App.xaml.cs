using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Symbiote.Caliburn;
using Symbiote.Core;

namespace Caliburn.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override object CreateRootModel()
        {
            return Container.GetInstance<IShellViewModel>();
        }

        public App()
        {
            Assimilate
                .Core()
                .Caliburn()
                .StartApplication();
        }
    }
}
