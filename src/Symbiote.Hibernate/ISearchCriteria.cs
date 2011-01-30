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
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Symbiote.Hibernate
{
    public interface ISearchCriteria<T>
    {
        int? PageNumber { get; }
        int? PageSize { get; }
        IEnumerable<Expression<Func<T, bool>>> All { get; }
        IEnumerable<Tuple<string, SortOrder>> Order { get; }

        ISearchCriteria<T> Add( Expression<Func<T, bool>> criteria );
        ISearchCriteria<T> PageBy( int pageNumber, int pageSize );
        ISearchCriteria<T> OrderBy<TProperty>( Expression<Func<T, TProperty>> orderBy, SortOrder order );
    }
}