using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Log;
using Symbiote.Daemon;
using Symbiote.Log4Net;
using Symbiote.Eidetic;
using Symbiote.Eidetic.Extensions;
using Symbiote.Relax;

namespace Demo
{
    class Symbiote_Core_1
    {
        static void Main(string[] args)
        {
            // Leaving Eidetic in place will allow us
            // to use memcached along with the Relax Symbiote
            // which is my CouchDB API
            Assimilate
                .Core()
                .Daemon<DemoService>(x =>  
                    x.Name("demoService") 
                     .DisplayName("A Demo Service") 
                     .Description("A demo service, how much clearer can this get?")
                     .Arguments(args) 
                    )
                .Eidetic(x => // configuration for memcached client
                        x.AddLocalServer() // uses localhost for server name
                         .DeadTimeOut(TimeSpan.FromSeconds(20)) // default is 30 seconds
                         .TimeOut(TimeSpan.FromSeconds(5)) // default is 10 seconds
                    )
                .Relax(x => 
                    x.UseDefaults() // the defaults are: http, localhost, port 5984, no auth
                     .Cache() // tells Relax to use memcached for caching
                              // CAUTION: this call without a preceding Eidetic() call
                              // will throw an exception!
                    )
                .RunDaemon<DemoService>(); // the LAST assimilate call to make, it starts the service
                
        }

        public static void JSONDemo()
        {
            var instance = new JsonTest() {Number = 10, Text = "text"};
            var json1 = instance.ToJson(); // includes type tokens in the json
            var json2 = instance.ToJson(false); // exclude type tokens in json

            var result1 = json1.FromJson(); // result1 is an object but of type JsonTest
            var result2 = json2.FromJson(); // w/o type token in json2, results in JObject
            var result3 = json2.FromJson<JsonTest>(); // result3 is JsonTest b/c of the generic parameter
        }


    }

    // Inheriting from DefaultCouchDocument allows you to 
    // persist a class to CouchDB via Relax.
    // The DocumentId property will be a Guid and the
    // DocumentRevision property will be a string
    [Serializable] // you need this if you want it to work with caching
    public class Simple : DefaultCouchDocument
    {
        public string Message { get; set; }

        // this will cause CouchDB to use
        // a GUID as the message id but it's
        // a little odd to do it this way...
        public Simple(string message)
        {
            Message = message;
            DocumentId = Guid.NewGuid().ToString();
        }
    }

    // Inheriting from CouchDocument allows you to 
    // choose the types of the key and revision.
    // The DocumentId property will be a Guid and the
    // DocumentRevision property will be a string
    [Serializable] // you need this if you want it to work with caching
    public class Custom : CouchDocument<Custom, Guid, string>
    {
        public string Message { get; set; }

        public Custom(string message)
        {
            Message = message;
            DocumentId = Guid.NewGuid();
        }
    }



    // Relax has two interfaces you can take
    // dependency on: ICouchServer and IDocumentRepository
    // You can get to IDocumentRepository FROM ICouchServer's
    // Repository property.
    // To have server-level API calls available, I
    // prefer taking dependency on ICouchServer
    public class DemoService : IDaemon
    {
        private ICouchServer _server;

        public void Start()
        {
            // Lets make a document...
            var document = new Simple("Hey! I'm a message! WOOHOO!");
            // and save it
            _server.Repository.Save(document);

            // but wait!? How does it know what Id to use?
            // how does it know WHAT DB to store it in?
            // Relax will use the type's name (converted to lowercase)
            // as the DB name and DefaultCouchDocuments handle DocumentId creation

            // let's get the document back
            var copy = _server.Repository.Get<Simple>(document.DocumentId);

            // let's change the message and update it
            copy.Message = "I'm still a message, just a little more serious.";
            _server.Repository.Save(copy);

            // let's add several documents...
            var newDocuments = new[]
                    {
                        new Simple("Document 1"),
                        new Simple("Document 2"),
                        new Simple("Document 3"),
                        new Simple("Document 4"),
                        new Simple("Document 5"),
                        new Simple("Document 6"),
                        new Simple("Document 7"),
                    };
            _server.Repository.SaveAll(newDocuments);

            // and get them back again...
            var list = _server.Repository.GetAll<Simple>();

            // let's show off paging...
            var page2 = _server.Repository.GetAll<Simple>(2, 2); // get's Document 3 & 4
        }

        public void Stop()
        {
            // we can delete the database now that we're done...
            _server.DeleteDatabase<Simple>();
        }

        public DemoService(ICouchServer server)
        {
            _server = server;
        }
    }

    public class JsonTest
    {
        public int Number { get; set; }
        public string Text { get; set; }
    }

    internal class Concrete : IDependency
    {
    }

    internal interface IDependency
    {
    }
}
