// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using FluentNHibernate.Cfg;
using NHibernate;
using Symbiote.Core;
using Symbiote.Hibernate.Impl;

namespace Symbiote.Hibernate
{
    public static class HibernateAssimilation
    {
        public static IAssimilate Hibernate( this IAssimilate assimilate, Action<HibernateConfigurator> config )
        {
            var configurator = new HibernateConfigurator( assimilate );
            config( configurator );
            return assimilate;
        }
    }

    public class HibernateConfigurator
    {
        private IAssimilate _assimilate;

        public HibernateConfigurator FromFactory( Action<FluentConfiguration> config )
        {
            var configuration = Fluently.Configure();
            config( configuration );
            _assimilate.Dependencies(
                x => x.For<ISessionFactory>().Use( configuration.BuildSessionFactory() ).AsSingleton() );
            return this;
        }

        public HibernateConfigurator( IAssimilate assimilate )
        {
            _assimilate = assimilate;
            assimilate.Dependencies( x =>
                                         {
                                             x.For<ISessionContext>().Use<InMemoryContext>();
                                             x.For<ISessionManager>().Use<SessionManager>();
                                             x.For<ISessionModule>().Use<SessionModule>();
                                             x.For( typeof( IRepository<> ) ).Use( typeof( Repository<> ) );
                                         } );
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
        }
    }
}