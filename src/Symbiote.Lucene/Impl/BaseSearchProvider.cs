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
using System.Collections.Generic;
using System.Linq.Expressions;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Symbiote.Core.Utility;

namespace Symbiote.Lucene.Impl
{
    public abstract class BaseSearchProvider : ILuceneSearchProvider
    {
        protected Searcher indexSearcher
        {
            get
            {
                var dirSearcher = new IndexSearcher(IndexWriter.GetDirectory(), true);
                var readerSearcher = new IndexSearcher(IndexWriter.GetReader());
                var searcher = new MultiSearcher(new Searchable[] {dirSearcher, readerSearcher});
                return searcher;
            }
        }

        public IndexWriter IndexWriter { get; set; }
        public Analyzer Analyzer { get; set; }

        public abstract IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery(string queryText);
        
        public virtual IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery<TModel>(Expression<Func<TModel, bool>> predicate)
        {
            DelimitedBuilder builder = new DelimitedBuilder("");
            ExpressionTreeProcessor.Process(predicate, "", builder);
            var query = builder.ToString();
            return GetDocumentsForQuery(query);
        }
    }
}