using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Machine.Specifications;
using Moq;
using StructureMap;
using Symbiote.Core;
using Symbiote.Net;
using It = Machine.Specifications.It;

namespace Net.Tests.Server
{
    public abstract class with_configuration
    {
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Core()
                                                .HttpServer(x => x.UseDefaults().UseBasicAuth());
                                        };
    }

    public abstract class with_server : with_configuration
    {
        protected static IHttpServer server;

        private Establish context = () =>
                                        {
                                            server = ObjectFactory.GetInstance<IHttpServer>();
                                        };
    }

    public abstract class with_watcher : with_server
    {
        protected static Mock<IObserver<HttpContext>> watcherMock;
        protected static IObserver<HttpContext> watcher;

        private Establish context = () =>
                                        {
                                            watcherMock = new Mock<IObserver<HttpContext>>();
                                            watcherMock.Setup(x => x.OnNext(Moq.It.IsAny<HttpContext>()));
                                            watcher = watcherMock.Object;
                                        };
    }

    public abstract class with_web_request : with_watcher
    {
        protected static WebRequest request;

        private Establish context = () =>
                                        {
                                            request = WebRequest.Create(@"http://localhost:8420");
                                            request.Credentials = new NetworkCredential("alex", "4l3x");
                                            request.PreAuthenticate = true;
                                        };
    }

    public class when_authenticating_valid_client_credentials : with_web_request
    {
        private static WebResponse response;

        private Because of = () =>
                                 {
                                     server.Start();
                                     response = request.GetResponse();
                                 };

        private It should_not_receive_null_response = () => response.ShouldNotBeNull();
    }
    
}
