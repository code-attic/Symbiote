using System;
using System.Diagnostics;
using Symbiote.Core;

namespace Symbiote.Daemon.Host
{
    [DebuggerDisplay("Service {Name} is {State}")]
    public class ServiceController<TService> :
        IServiceController
        where TService : class, IDaemon
    {
        protected bool Initialized { get; set; }
        protected TService ServiceInstance { get; set; }
        protected bool Disposed { get; set; }
        public string Name { get; private set; }
        public Type ServiceType { get { return typeof(TService); } }
        public ServiceState State { get; protected set; }

        //public ServiceController(string serviceName)
        //{
        //    Name = serviceName;
        //}

        public ServiceController(TService service)
        {
            ServiceInstance = service;
        }

        public Action<TService> PauseAction { get; set; }
        public Action<TService> ContinueAction { get; set; }

        #region Dispose Stuff

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            if (Disposed)
                return;

            ServiceInstance = default(TService);
            PauseAction = null;
            ContinueAction = null;
            Disposed = true;
        }

        ~ServiceController()
        {
            Dispose(false);
        }

        #endregion

        public void Initialize()
        {
            if (Initialized)
                return;

            try
            {
                ServiceInstance = Assimilate.GetInstanceOf<TService>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            Initialized = true;
        }

        public void Start()
        {
            try
            {
                ServiceInstance.Start();
            }
            catch (Exception ex)
            {
                SendFault(ex);
            }
        }

        public void Stop()
        {
            try
            {
                ServiceInstance.Stop();
            }
            catch (Exception ex)
            {
                SendFault(ex);
            }
        }

        public void Pause()
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                SendFault(ex);
            }
        }

        public void Continue()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                SendFault(ex);
            }
        }

        void SendFault(Exception exception)
        {
            try
            {
                
            }
            catch (Exception)
            {
                
            }
        }
    }
}