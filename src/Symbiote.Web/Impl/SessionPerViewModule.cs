using System;
using System.Web;
using Symbiote.Hibernate;

namespace Symbiote.Web.Impl
{
    public class SessionPerViewModule : IHttpModule
    {
        private readonly ISessionModule _module;

        public SessionPerViewModule(ISessionModule module)
        {
            _module = module;
        }

        #region IHttpModule Members

        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequest;
            context.EndRequest += EndRequest;
        }

        public virtual void EndRequest(object sender, EventArgs e)
        {
            _module.EndSession();
        }

        public virtual void BeginRequest(object sender, EventArgs e)
        {
            _module.BeginSession();
        }


        public void Dispose()
        {
            _module.Dispose();
        }

        #endregion
    }
}