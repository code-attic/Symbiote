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
using System.Collections.Generic;
using Symbiote.Couch.Config;
using Symbiote.Couch.Impl.Cache;

namespace Symbiote.Couch.Impl.Repository
{
    public class CachedDocumentRepository
        : BaseDocumentRepository
    {
        protected ICacheKeyBuilder _builder;
        protected ICouchCacheProvider _cache;
        protected IDocumentRepository _repository;

        public override void DeleteDocument<TModel>( object id, string rev )
        {
            _cache.Delete<TModel>( id, rev, base.DeleteDocument<TModel> );
        }

        public override void DeleteDocument<TModel>( object id )
        {
            _cache.Delete<TModel>( id, base.DeleteDocument<TModel> );
        }

        public override TModel Get<TModel>( object id, string revision )
        {
            return _cache.Get( id, revision, base.Get<TModel> );
        }

        public override TModel Get<TModel>( object id )
        {
            return _cache.Get( id, base.Get<TModel> );
        }

        public override IList<TModel> GetAll<TModel>()
        {
            return _cache.GetAll( base.GetAll<TModel> );
        }

        public override IList<TModel> GetAll<TModel>( int pageSize, int pageNumber )
        {
            return _cache.GetAllPaged( pageNumber, pageSize, base.GetAll<TModel> );
        }

        public override IList<TModel> GetAllBetweenKeys<TModel>( object startingWith, object endingWith )
        {
            return _cache.GetAllInRange( startingWith, endingWith, base.GetAllBetweenKeys<TModel> );
        }

        public override void Save<TModel>( TModel model )
        {
            _cache.Save( model, base.Save );
        }

        public override void SaveAll<TModel>( IEnumerable<TModel> list )
        {
            _cache.SaveAll( list, base.SaveAll );
        }

        public CachedDocumentRepository( ICouchCacheProvider cacheProvider, ICouchConfiguration configuration )
            : base( configuration )
        {
            _cache = cacheProvider;
        }
    }
}