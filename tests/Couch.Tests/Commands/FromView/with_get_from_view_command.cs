using Machine.Specifications;
using Symbiote.Couch.Impl.Commands;
using Symbiote.Couch.Impl.Http;

namespace Couch.Tests.Commands.FromView
{
    public abstract class with_get_from_view_command : with_command_factory
    {
        protected static string url;
        protected static GetFromViewCommand command;
        protected static string jsonResult;

        private Establish context = () =>
                                        {
                                            jsonResult = @"
{
    total_rows: 2,
    offset: 0,
    rows:
        [
            {
                doc: { _id: ""test"", _rev: ""1"", Message: ""Hi"" }
            },
            {
                doc: { _id: ""_design/mydesigndoc"" }
            }
        ]
}
";
                                            url = @"http://localhost:5984/symbiotecouch/_design/test/_view/test?stale=ok&reduce=false";
                                            mockAction
                                                .Setup(x => x.Get(Moq.It.Is<CouchUri>(u => u.ToString() == url)))
                                                .Returns(jsonResult);

                                            command = factory.CreateGetFromViewCommand();
                                        };
    }
}
