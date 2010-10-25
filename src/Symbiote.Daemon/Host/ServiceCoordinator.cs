using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core;
using Symbiote.Core.Extensions;

namespace Symbiote.Daemon.Host
{
    public class ServiceInformation
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public ServiceState State { get; set; }
    }

    [DebuggerDisplay("Hosting {HostedServiceCount} Services")]
    public class ServiceCoordinator :
        IServiceCoordinator
    {
        protected IList<IServiceController> Services { get; set; }
        protected DaemonConfiguration Configuration { get; set; }

        public ServiceCoordinator(DaemonConfiguration configuration)
        {
            Services = new List<IServiceController>();
            Configuration = configuration;
            RegisterServices();
        }

        public void Start()
        {
            Services.ForEach(x => x.Start());
        }

        public void Stop()
        {
            Services.ForEach(x => x.Stop());
        }

        public void Pause()
        {
            
        }

        public void Continue()
        {
            
        }

        public void StartService(string name)
        {
            Services.Where(x => x.Name.Equals(name)).First().Start();
        }

        public void StopService(string name)
        {
            Services.Where(x => x.Name == name).First().Stop();
        }

        public void PauseService(string name)
        {
            Services.Where(x => x.Name == name).First().Pause();
        }

        public void ContinueService(string name)
        {
            Services.Where(x => x.Name == name).First().Continue();
        }

        public int HostedServiceCount
        {
            get { return Services.Count; }
        }

        public IList<ServiceInformation> GetServiceInfo()
        {
            return Services
                .ToList()
                .ConvertAll(serviceController => new ServiceInformation
                                                     {
                                                         Name = serviceController.Name,
                                                         State = serviceController.State,
                                                         Type = serviceController.ServiceType.Name
                                                     });
        }

        public IServiceController GetService(string name)
        {
            return Services.Where(x => x.Name == name).FirstOrDefault();
        }

        #region Dispose

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                Services.ForEach(s => s.Dispose());
                Services.Clear();
            }
            _disposed = true;
        }

        ~ServiceCoordinator()
        {
            Dispose(false);
        }

        #endregion

        public void AddNewService(IServiceController controller)
        {
            Services.Add(controller);
        }

        public void RegisterServices()
        {
            Assimilate
                .Assimilation
                .DependencyAdapter
                .GetTypesRegisteredFor<IDaemon>()
                .Select(x => typeof (ServiceController<>).MakeGenericType(x))
                .Select(CreateController)
                .ForEach(Services.Add);
        }

        protected IServiceController CreateController(Type type)
        {
            return Assimilate.GetInstanceOf(type) as IServiceController;
        }
    }
}