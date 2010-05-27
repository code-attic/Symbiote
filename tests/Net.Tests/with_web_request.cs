using System;
using System.Net;
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
                                            request.Credentials = new NetworkCredential("alex", "4l3x");
                                            request.PreAuthenticate = true;
                                            request.BeginGetResponse(onResponse, null);
                                        };

        private static void onResponse(IAsyncResult ar)
        {
            var response = request.EndGetResponse(ar);
        }
    }
}