using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Machine.Specifications;
using Version = Lucene.Net.Util.Version;

namespace Lucene.Tests
{
    public class Person
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual int Age { get; set; }
        public virtual DateTime DateOfBirth { get; set; }
        public virtual List<Car> Cars { get; set; }
    }

    public class Car
    {
        public virtual string Make { get; set; }
        public virtual string Model { get; set; }
    }

    public class when_searching_against_stored_index : with_index_on_disk
    {
        protected static List<Tuple<ScoreDoc, Document>> documents;
        protected static Stopwatch watch;
        protected static int age;

        private Because of = () =>
                                 {
                                     watch = Stopwatch.StartNew();
                                     documents = searchProvider.GetDocumentsForQuery<Person>(x => 
                                        x.Age == 31).ToList();
                                     watch.Stop();

                                     age = int.Parse(documents[0].Item2.Get("Age"));
                                 };
        
        private It should_return_one_document = () =>
                                                documents.Count.ShouldEqual(1);
        
        private It should_have_age_31 = () => age.ShouldEqual(31);
    }
}