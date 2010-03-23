using NHibernate;

namespace Symbiote.Hibernate.Impl
{
    public class SessionManager : ISessionManager
    {
        private readonly ISessionContext _context;

        public SessionManager(ISessionContext context)
        {
            _context = context;
        }

        public ISession CurrentSession
        {
            get
            {
                if (_context.Contains("CurrentSession"))
                {
                    return _context.Get("CurrentSession") as ISession;
                }
                else
                {
                    return null;
                }
            }
            set { _context.Set("CurrentSession", value); }
        }
    }
}