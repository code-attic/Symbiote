using NHibernate;

namespace Symbiote.Hibernate
{
    public class SessionManager : ISessionManager
    {
        private readonly IContext _context;

        public SessionManager(IContext context)
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

        private SessionManager()
        {
            
        }
    }
}