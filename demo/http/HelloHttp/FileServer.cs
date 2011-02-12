
using System;
using System.Collections.Generic;
using System.IO;
using Symbiote.Http;
using Symbiote.Http.Impl;

namespace HelloHttp
{
    public class FileServer : IApplication
    {
        public void Process(IDictionary<string, object> requestItems, OwinResponse respond, Action<Exception> onException)
        {

            //After helper
            var request = requestItems.ExtractRequest();
            respond
                .Build()
                .AppendFileContentToBody( request.Url )
                .Submit( HttpStatus.Ok );

            //Before Helper

            //var url = requestItems[Owin.ItemKeys.REQUEST_URI].ToString().Replace("/", @"\");
            //var filePath = @"..\.." + url;

            //if(!File.Exists(filePath))
            //{
            //    respond(
            //        Owin.HttpStatus.NOTFOUND,
            //        new Dictionary<string, IList<string>>()
            //            {
            //                {"content-type", new List<string>() {"text/html"}}
            //            },
            //        new[] { "DAMMIT JIM, I'm a file server, not a magician!" });
            //}

            //using(var memoryStream = new MemoryStream())
            //using(var stream = new FileStream(filePath, FileMode.Open ))
            //{
            //    var bytes = new byte[stream.Length];
            //    stream.Read(bytes, 0, (int) stream.Length);
             
            //    respond(
            //        Owin.HttpStatus.OK,
            //        new Dictionary<string, IList<string>>()
            //            {
            //                {"content-type", new List<string>() {"text/html"}}
            //            },
            //        new [] {bytes});
            //}
        }
    }
}
