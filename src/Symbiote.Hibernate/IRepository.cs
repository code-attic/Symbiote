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

namespace Symbiote.Hibernate
{
    public interface IRepository<T> where T : class
    {
        void Commit();
        T FindOne<TEntity>( ISearchCriteria<TEntity> entity ) where TEntity : T;
        IList<T> FindAll<TEntity>( ISearchCriteria<TEntity> entity ) where TEntity : T;
        void Delete<TEntity>( object id ) where TEntity : T;
        void Delete<TEntity>( ISearchCriteria<TEntity> criteria ) where TEntity : T;
        T Get<TEntity>( object id ) where TEntity : T;
        IList<T> GetAll<TEntity>( ISearchCriteria<TEntity> criteria ) where TEntity : T;
        int GetCount<TEntity>( ISearchCriteria<TEntity> criteria ) where TEntity : T;
        int GetTotal<TEntity>() where TEntity : T;
        void Insert<TEntity>( TEntity entity ) where TEntity : T;
        void Insert<TEntity>( IEnumerable<TEntity> list ) where TEntity : T;
    }
}