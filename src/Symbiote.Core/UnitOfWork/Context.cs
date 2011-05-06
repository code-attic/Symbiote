// /* 
// Copyright 2008-2011 Jim Cowart & Alex Robson
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

namespace Symbiote.Core.UnitOfWork
{
    public class Context
    {
        private static IContextProvider _provider;

        protected static IContextProvider ContextProvider
        {
            get
            {
                _provider = _provider ?? Assimilate.GetInstanceOf<IContextProvider>();
                return _provider;
            }
        }

        /// <summary>
        /// Creates a unit of work context for a given Actor (Domain Object)
        /// </summary>
        /// <typeparam name="TActr">Actor must be a reference type - this should typically be a domain object type</typeparam>
        /// <param name="instance">The actual instance of your domain object</param>
        /// <returns>IContext</returns>
        public static IContext<TActr> CreateFor<TActr>( TActr instance )
            where TActr : class
        {
            return ContextProvider.GetContext( instance );
        }

        /// <summary>
        /// Creates a unit of work context for a given Actor (Domain Object) and immediately executes the unit of work,
        /// with the actual "work" being the action passed in as the onCommit.  An action passed for onSuccess will fire when the
        /// Commit succeeds, action passed for onException fires if the Commit throws an exception.  This method simply
        /// provides more terse (and perhaps sugary) syntax to keep you from have to code up a "using" block.
        /// </summary>
        /// <typeparam name="TActr">Reference Type that should typically be a domain object type.</typeparam>
        /// <param name="instance">The actual instance of your domain object</param>
        /// <param name="onCommit">The Action that defines the work being done to the Actor</param>
        /// <param name="onSuccess">An Action to take once the work succeeds.</param>
        /// <param name="onException">An Action to take if the work throws an exception.</param>
        public static void CreateAndExecute<TActr>( TActr instance, Action<TActr> onCommit, Action<TActr> onSuccess, Action<TActr, Exception> onException)
            where TActr : class
        {
            using(var context = CreateFor( instance )
                .OnCommit( onCommit )
                .OnSuccess( onSuccess )
                .OnException( onException ))
            {
            }
        }
    }
}