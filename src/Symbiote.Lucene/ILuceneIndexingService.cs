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
using Symbiote.Core;
using System.Linq;

namespace Symbiote.Lucene
{
    public interface ILuceneIndexingService
    {
        void IndexObservable<TObservable, TEvent>( string indexName, TObservable observable )
            where TObservable : IObservable<TEvent>;
    }

    public class LuceneIndexingService
        : ILuceneIndexingService
    {
        protected ILuceneServiceFactory serviceFactory { get; set; }
        protected Action<string> writeIndexToDisk { get; set; }

        public void IndexObservable<TObservable, TEvent>( string indexName, TObservable observable )
            where TObservable : IObservable<TEvent>
        {
            var indexer = serviceFactory.GetIndexingObserverForIndex( indexName );
            observable
                .Subscribe( x => IndexEvent( indexer, x ) );

            observable
                .BufferWithTime( TimeSpan.FromMinutes( 2 ) )
                .Do( x => WriteIndex( indexName ) );
        }

        protected void IndexEvent<T>( ILuceneIndexer indexer, T newEvent )
        {
            var visitor = Assimilate.GetInstanceOf<IVisit<T>>();
            visitor.Subscribe( indexer );
            visitor.Accept( newEvent );
        }

        protected void WriteIndex( string index )
        {
            var writer = serviceFactory.GetIndexWriter( index );
            writer.Commit();
        }

        public LuceneIndexingService( ILuceneServiceFactory serviceFactory )
        {
            this.serviceFactory = serviceFactory;
            writeIndexToDisk = WriteIndex;
        }
    }
}