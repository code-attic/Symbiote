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
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Symbiote.Core;
using Symbiote.Eidetic.Config;

namespace Symbiote.Eidetic
{
    public static class EideticAssimilation
    {
        public static IAssimilate Eidetic( this IAssimilate assimilate, Action<EideticConfigurator> configure )
        {
            var config = new EideticConfigurator();
            configure( config );
            assimilate.Dependencies( x => x.For<IMemcachedClientConfiguration>()
                                              .Use( config.Configuration ) );

            var clientConfig = Assimilate.GetInstanceOf<IMemcachedClientConfiguration>();
            assimilate.Dependencies(
                x => x.For<MemcachedClient>().Use( new MemcachedClient( clientConfig ) ).AsSingleton() );
            return assimilate;
        }
    }
}