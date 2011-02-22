using System;
using Couch.Tests.Commands;
using Machine.Specifications;

namespace Couch.Tests.Repository
{
    public class when_deleting_attachment : with_delete_attachment_command
    {
        private static Exception exception;

        private Because of = () =>
                                 {
                                     exception =
                                         Catch.Exception(
                                             () => repository.DeleteAttachment<TestDoc>(document, attachmentName));
                                 };

        private It should_delete_attachment_without_exception = () => exception.ShouldBeNull();
        private It should_call_command_correctly = () => commandMock.VerifyAll();
    }
}