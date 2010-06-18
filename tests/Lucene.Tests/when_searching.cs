using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Machine.Specifications;
using Symbiote.Core.Extensions;
using Symbiote.Lucene.Impl;
using Version = Lucene.Net.Util.Version;

namespace Lucene.Tests
{
    public class when_searching : with_lucene
    {
        protected static List<Tuple<ScoreDoc, Document>> documents;
        protected static Stopwatch watch;
        protected static int age;
        
        private Because of = () =>
                                 {
                                     watch = Stopwatch.StartNew();
                                     documents = searchProvider.GetDocumentsForQuery("Age:31").ToList();
                                     watch.Stop();

                                     age = int.Parse(documents[0].Item2.Get("Age"));
                                 };
        
        private It should_return_one_document = () => 
                                                documents.Count.ShouldEqual(1);
        
        private It should_have_age_31 = () => age.ShouldEqual(31);
    }
}