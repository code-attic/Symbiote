using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;

namespace Symbiote.Daemon.Host
{
    public class HostRunner
    {
        static HostRunner()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            "Host encountered an unhandled exception on the AppDomain"
                .ToFatal<IHost>((Exception) e.ExceptionObject);
        }

        public static void Start(IHost host)
        {
            host.Start();
        }
    }
}
