using System;
using System.IO;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Net;

namespace Net.Tests
{
    public class when_receiving_web_request : with_web_request
    {
        protected static string request;

        private Because of = () =>
                                 {
                                     while(secureStream == null || !secureStream.CanRead)
                                     {
                                         Thread.Sleep(1000);
                                     }

                                     var reader = new HttpStreamReader(secureStream);
                                     var watcher = new HttpStreamWatcher(); 
                                     reader.Subscribe(watcher);
                                     reader.Start();

                                     request = watcher.GetMessage();

                                     using(var writer = new StreamWriter(secureStream))
                                     {
                                         writer.WriteLine("BOOGABOOGA!");
                                     }
                                 };
        
        private It should_have_request_body = () => request.ShouldNotBeEmpty();
    }
}