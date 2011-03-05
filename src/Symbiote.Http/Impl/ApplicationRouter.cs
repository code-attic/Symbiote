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
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core.DI;
using Symbiote.Core.Extensions;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl
{
    public class ApplicationRouter :
        IRegisterApplication,
        IRouteRequest
    {
        public List<Tuple<Predicate<IRequest>, IApplicationFactory>> Routes { get; set; }

        public IRegisterApplication DefineApplication<TApplication>( Predicate<IRequest> route )
            where TApplication : IApplication
        {
            IApplicationFactory applicationFactory = new ApplicationFactory<TApplication>();
            Routes.Add( Tuple.Create( route, applicationFactory ) );
            return this;
        }

        public IRegisterApplication DefineApplication( Predicate<IRequest> route, Type applicationType )
        {
            if ( !applicationType.ImplementsInterfaceTemplate( typeof( IApplication ) ) )
            {
                throw new ArgumentException(
                    "{0} is not a valid IApplication implementation. Applications must implement IApplication interface." );
            }

            IApplicationFactory applicationFactory = new ApplicationFactory( applicationType );
            Routes.Add( Tuple.Create( route, applicationFactory ) );
            return this;
        }

        public IRegisterApplication DefineApplication( Predicate<IRequest> route, OwinApplication application )
        {
            IApplicationFactory applicationFactory = new DelegateApplicationFactory( application );
            Routes.Add( Tuple.Create( route, applicationFactory ) );
            return this;
        }

        public IApplication GetApplicationFor( IRequest request )
        {
            var route = Routes.FirstOrDefault( x => x.Item1( request ) );
            if ( route == null )
            {
                throw new MissingApplicationException(
                    "No application route existed for request: {0}".AsFormat( request.BaseUri ) );
            }
            return route.Item2.CreateApplication();
        }

        public ApplicationRouter()
        {
            Routes = new List<Tuple<Predicate<IRequest>, IApplicationFactory>>();
        }
    }
}