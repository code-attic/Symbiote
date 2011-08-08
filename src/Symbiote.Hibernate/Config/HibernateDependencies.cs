using System;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Hibernate.Impl;

namespace Symbiote.Hibernate
{
    public class HibernateDependencies : IDefineDependencies
    {
        public Action<DependencyConfigurator> Dependencies()
        {
            return container => 
                       {
                           container.For<ISessionContext>().Use<InMemoryContext>();
                           container.For<ISessionManager>().Use<SessionManager>();
                           container.For<ISessionModule>().Use<SessionModule>();
                           container.For( typeof( IRepository<> ) ).Use( typeof( Repository<> ) );
                       };
        }
    }
}