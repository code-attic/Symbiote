using System;
using NHibernate;

namespace Symbiote.Hibernate
{
    public interface ISessionManager
    {
        ISession CurrentSession { get; set; }
    }
}