using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Core.Tests
{
    public interface IService
    {
        string GetMessage();
    }



    public class with_my_context
    {
        protected static string message { get; set; }
        protected static IService service { get; set; }
        protected static Mock<IService> serviceMock { get; set; }

        private Establish context = () =>
                                        {
                                            serviceMock = new Mock<IService>();
                                            service = serviceMock.Object;
                                        };
    }

    public class with_service_mock : with_my_context
    {
        private Establish context = () =>
                                        {
                                            serviceMock
                                                .Setup(x => x.GetMessage())
                                                .Returns("Hi, it's a message")
                                                .AtMostOnce();
                                        };
    }

    public class when_there_is_a_message : with_service_mock
    {
        protected static bool stringIsEmpty;
        private Because of = () =>
                                 {
                                     var message = service.GetMessage();
                                     stringIsEmpty = string.IsNullOrEmpty(message);
                                 };

        private It message_should_not_be_empty = () => stringIsEmpty.ShouldBeFalse();
        private It should_call_service_one_time = () => serviceMock.Verify(x => x.GetMessage(), Times.Exactly(2));
    }
}
