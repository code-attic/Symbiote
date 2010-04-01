using System;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Extensions;
using It = Machine.Specifications.It;

namespace Relax.Tests.Repository
{
    public class when_creating_database : with_create_database_command
    {
        private static Exception exception = null;
        private Because of = () =>
                                 {
                                     exception = Catch.Exception(
                                         () => repository.CreateDatabase()
                                     );
                                 };

        private It should_create_database_without_exception = () => exception.ShouldBeNull();
        private It should_call_put_correctly = () => commandMock.Verify(x => x.Put(couchUri));
    }
}
