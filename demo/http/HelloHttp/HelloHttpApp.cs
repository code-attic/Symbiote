using System;
using System.Collections.Generic;
using Symbiote.Http;
using Symbiote.Http.Impl;

namespace HelloHttp
{
    public class HelloHttpApp : IApplication
    {
        public void Process(IDictionary<string, object> requestItems, Action<string, IDictionary<string, IList<string>>, IEnumerable<object>> respond, Action<Exception> onException)
        {
            respond
                .Build()
                .DefineHeaders( x => x.ContentType( ContentType.Plain ) )
                .AppendToBody( "Hi http, this is an OWIN compliant host!" )
                .Submit( HttpStatus.Ok );

            // Before Helper
            //respond(
            //    Owin.HttpStatus.OK,
            //    new Dictionary<string, IList<string>>(),
            //    new[] { "Hi Http, this is a OWIN compliant host!" });
        }
    }
}