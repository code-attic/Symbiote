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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core.Extensions;
using Symbiote.Core.Reflection;

namespace Symbiote.Core.Utility
{
    public class HierarchyVisitor
        : IObservable<Tuple<object, string, object>>
    {
        protected readonly string pathTemplate = "{0}.{1}";
        protected List<IObserver<Tuple<object, string, object>>> Watchers { get; set; }
        protected Predicate<object> MatchIdentifier { get; set; }
        protected bool IncludePath { get; set; }

        #region IObservable<Tuple<object,string,object>> Members

        public IDisposable Subscribe( IObserver<Tuple<object, string, object>> observer )
        {
            var disposable = observer as IDisposable;
            Watchers.Add( observer );
            return disposable;
        }

        #endregion

        public void Visit( object instance )
        {
            if ( MatchIdentifier( instance ) )
                NotifyWatchers( null, "", null, instance );

            if ( IsCrawlableEnumerable( instance ) )
                Visit( null, "", "", instance );
            else
                Crawl( "", instance );
        }

        protected void Crawl( string path, object instance )
        {
            Reflector
                .GetProperties( instance.GetType() )
                .Where( x => !x.PropertyType.IsValueType )
                .ForEach( x => Visit( instance, path, x.Name, Reflector.ReadMember( instance, x.Name ) ) );
        }

        protected void Visit( object parent, string path, string member, object value )
        {
            if ( value != null )
            {
                if ( IsCrawlableEnumerable( value ) )
                {
                    try
                    {
                        var enumerator = (value as IEnumerable).GetEnumerator();
                        enumerator.MoveNext();
                        do
                        {
                            if ( enumerator.Current != null )
                                Visit( parent, path, member, enumerator.Current );
                        } while ( enumerator.MoveNext() );
                    }
                    catch ( Exception e )
                    {
                        Console.WriteLine( e );
                    }
                }
                else
                {
                    path = string.IsNullOrWhiteSpace( path )
                               ? member
                               : pathTemplate.AsFormat( path, member );

                    if ( MatchIdentifier( value ) )
                        NotifyWatchers( parent, path, member, value );
                    Crawl( path, value );
                }
            }
        }

        protected bool IsCrawlableEnumerable( object value )
        {
            return value.GetType().GetInterface( "IEnumerable", false ) != null && value.GetType().Name != "String";
        }

        protected void NotifyWatchers( object parent, string path, string member, object instance )
        {
            Watchers.ForEach( x => x.OnNext( GetTuple( parent, path, member, instance ) ) );
        }

        public Tuple<object, string, object> GetTuple( object parent, string path, string member, object instance )
        {
            return Tuple.Create(
                parent,
                IncludePath ? pathTemplate.AsFormat( path, member ) : member,
                instance
                );
        }

        public HierarchyVisitor( bool includePath, Predicate<object> notifyOnMatches )
        {
            IncludePath = includePath;
            MatchIdentifier = notifyOnMatches;
            Watchers = new List<IObserver<Tuple<object, string, object>>>();
        }
    }
}