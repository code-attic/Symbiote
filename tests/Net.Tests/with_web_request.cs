using System;
using System.IO;
using System.Net;
using System.Threading;
using Machine.Specifications;

namespace Net.Tests
{
    public abstract class with_web_request : with_tcp_listener
    {
        protected static WebRequest request;
        protected static WebResponse response;

        private Establish context = () =>
                                        {
                                            ServicePointManager.ServerCertificateValidationCallback = validateCert;
                                            request = WebRequest.Create("https://localhost:8001");
                                            //request.Credentials = new NetworkCredential("alex", "4l3x");
                                            //request.PreAuthenticate = true;
                                            request.Method = "POST";

                                            var async = request.BeginGetRequestStream(OnRequestStream, null);
                                            async.AsyncWaitHandle.WaitOne();
                                            Thread.Sleep(1000);
                                            request.BeginGetResponse(onResponse, null);
                                        };

        private static void OnRequestStream(IAsyncResult ar)
        {
            var reqStream = request.EndGetRequestStream(ar);
            var writer = new StreamWriter(reqStream);
            writer.Write(@"{
    id: 10,
    howdy: ""hi""
}");
            writer.Flush();
        }

        private static void onResponse(IAsyncResult ar)
        {
            var response = request.EndGetResponse(ar);
        }
    }
}