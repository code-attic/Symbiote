﻿// /* 
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
using Symbiote.Core.Actor;
using Symbiote.Core.DI;
using Symbiote.Core.Locking;
using Symbiote.Core.Log;
using Symbiote.Core.Log.Impl;
using Symbiote.Core.Memento;
using Symbiote.Core.Serialization;
using Symbiote.Core.UnitOfWork;

namespace Symbiote.Core
{
    public class CoreDependencies : IDefineStandardDependencies
    {
        public Action<DependencyConfigurator> DefineDependencies()
        {
            return container =>
                {
                    container.For<ILogProvider>().Use<NullLogProvider>();
                    container.For<ILogger>().Use<NullLogger>();
                    container.For( typeof( ILogger<> ) ).Add( typeof( ProxyLogger<> ) );
                    container.For<ILockManager>().Use<NullLockManager>();
                    container.For( typeof( KeyAccessAdapter<> ) ).Use( typeof( KeyAccessAdapter<> ) );
                    container.For<IKeyAccessor>().Use<KeyAccessManager>().AsSingleton();
                    container.For<IMemoizer>().Use<Memoizer>();
                    container.For(typeof( IMemento<> ) ).Use( typeof( PassthroughMemento<> ) );
                    container.For<IEventPublisher>().Use<EventPublisher>().AsSingleton();
                    container.For<IContextProvider>().Use<DefaultContextProvider>();
                    container.For<IEventConfiguration>().Use<EventConfiguration>();
                    container.For<IJsonSerializerFactory>().Use<JsonSerializerFactory>().AsSingleton();
                    container.For<IDependencyAdapter>().Use( Assimilate.Assimilation.DependencyAdapter );
                    container.For<IEventListenerManager>().Use<EventListenerManager>().AsSingleton();
                    container.For( typeof( IAgentFactory ) ).Use<DefaultAgentFactory>();
                    container.For( typeof( IAgent<> ) ).Use( typeof( DefaultAgent<> ) ).AsSingleton();
                    container.For<IAgency>().Use<Agency>().AsSingleton();
                    container.For( typeof( IActorCache<> ) ).Use( typeof( NullActorCache<> ) );
                    container.For( typeof( IActorStore<> ) ).Use( typeof( NullActorStore<> ) );
                    container.For( typeof( IActorFactory<> ) ).Use( typeof( DefaultActorFactory<> ) );
                };
        }
    }
}
