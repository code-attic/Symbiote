using System;
using System.Threading;
using Symbiote.Core.Extensions;

namespace Symbiote.Daemon.Host
{
    public class SimpleHost
        : IHost
    {
        readonly IServiceCoordinator ServiceCoordinator;
        readonly ServiceName Name;

        public SimpleHost(ServiceName name, IServiceCoordinator serviceCoordinator)
        {
            Name = name;
            ServiceCoordinator = serviceCoordinator;
        }

        public void Start()
        {
            CheckToSeeIfWinServiceRunning();
            "Daemon loading in console...".ToDebug<IHost>();
            var externalTriggeredTerminatation = new ManualResetEvent(false);
            var waitHandles = new WaitHandle[] { externalTriggeredTerminatation};

            Console.CancelKeyPress += delegate
                                          {
                                              "Control+C detected, exiting.".ToInfo<IHost>();
                                              ServiceCoordinator.Stop();
                                              ServiceCoordinator.Dispose();
                                              externalTriggeredTerminatation.Set();
                                          };

            ServiceCoordinator.Start();
            "Daemon has started, press Control+C to exit."
                .ToInfo<IHost>();
            WaitHandle.WaitAny(waitHandles);
        }

        protected void CheckToSeeIfWinServiceRunning()
        {
            //if (ServiceController.GetServices().Where(s => s.ServiceName == Name.FullName).Any())
            //{
            //    "There is an instance of this {0} running as a windows service"
            //        .ToWarn<IHost>(Name);
            //}
        }
    }
}