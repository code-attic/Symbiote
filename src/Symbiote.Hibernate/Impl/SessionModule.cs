using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Symbiote.Hibernate.Impl
{
    public class SessionModule : ISessionModule
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly ISessionManager _sessionManager;

        public SessionModule(ISessionFactory factory, ISessionManager manager)
        {
            _sessionFactory = factory;
            _sessionManager = manager;
        }

        public virtual void BeginSession()
        {
            var session = _sessionFactory.OpenSession();
            session.BeginTransaction();
            _sessionManager.CurrentSession = session;
        }

        public virtual void EndSession()
        {
            var session = _sessionManager.CurrentSession;
            if (session != null && session.IsOpen)
            {
                if (session.Transaction.IsActive)
                {
                    session.Transaction.Commit();
                }
                session.Close();
            }
            if (session != null)
            {
                session.Dispose();
            }
        }

        public virtual void Dispose()
        {
            _sessionFactory.Dispose();
        }
    }
}
