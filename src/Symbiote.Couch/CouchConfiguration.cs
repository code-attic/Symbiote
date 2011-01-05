/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using Symbiote.Core;
using Symbiote.Core.Impl.Cache;
using Symbiote.Core.Impl.DI;
using Symbiote.Couch.Config;
using Symbiote.Couch.Impl;
using Symbiote.Couch.Impl.Cache;
using Symbiote.Couch.Impl.Http;
using Symbiote.Couch.Impl.Repository;
using Symbiote.Couch.Impl.Serialization;

namespace Symbiote.Couch
{
    public class CouchConfiguration
    {
        public static void Configure<TDepedencyAdapter>()
            where TDepedencyAdapter : class, IDependencyAdapter, new()
        {
            Assimilate.Core<TDepedencyAdapter>();
            var config = new CouchConfigurator();
            Configure(config.GetConfiguration());
        }

        public static void Configure<TDepedencyAdapter>(Action<CouchConfigurator> configure)
            where TDepedencyAdapter : class, IDependencyAdapter, new()
        {
            Assimilate.Core<TDepedencyAdapter>();
            var config = new CouchConfigurator();
            configure(config);
            Configure(config.GetConfiguration());
        }

        public static void Configure(IDependencyAdapter dependencyAdapter, Action<CouchConfigurator> configure)
        {
            Assimilate.Core(dependencyAdapter);
            var config = new CouchConfigurator();
            configure(config);
            Configure(config.GetConfiguration());
        }

        public static void Configure(ICouchConfiguration configuration)
        {
            Assimilate.Dependencies(c =>
            {
                c.For<ICouchConfiguration>().Use(configuration);
                c.For<IHttpAction>().Use<HttpAction>();
                c.For<ICouchCacheProvider>().Use<CouchCacheProvider>();
                c.For<ICacheKeyBuilder>().Use<CacheKeyBuilder>();
                c.For<ICouchServer>().Use<CouchDbServer>();
                c.For<IKeyAssociationManager>().Use<DefaultKeyAssociationManager>().AsSingleton();
                c.For<DocumentConventions>().Use(configuration.Conventions);
                if (configuration.Cache)
                {
                    if (!Assimilate.Assimilation.DependencyAdapter.HasPluginFor<ICacheProvider>())
                    {
                        throw new CouchConfigurationException(
                            "You must have an implementation of ICacheProvider configured to use caching in Couch. Consider referencing Symbiote.Eidetic and adding the .Eidetic() call before this in your assimilation to utilize memcached or memcachedb as the cache provider for Couch."
                        );
                    }

                    c.For<IDocumentRepository>().Use<CachedDocumentRepository>();
                }
                else
                {
                    c.For<IDocumentRepository>().Use<DocumentRepository>();
                }
            });
        }
    }
}
