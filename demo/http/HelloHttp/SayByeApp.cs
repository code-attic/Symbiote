using System;
using System.Collections.Generic;
using Symbiote.Http;
using Symbiote.Http.Impl;

namespace HelloHttp
{
    public class SayByeApp : IApplication
    {
        public void Process(IDictionary<string, object> requestItems, Action<string, IDictionary<string, IList<string>>, IEnumerable<object>> respond, Action<Exception> onException)
        {
            // After Helper
            respond
                .Build()
                .DefineHeaders( x => x.ContentType( ContentType.Plain ) )
                .AppendToBody( "See ya!" )
                .Submit( HttpStatus.Ok );

            // Before Helper
            //respond
            //    (
            //        Owin.HttpStatus.OK, 
            //        new Dictionary<string, IList<string>>(), 
            //        new[] { "See ya, {0}!".AsFormat( "dude" )}
            //    );
        }
    }
}