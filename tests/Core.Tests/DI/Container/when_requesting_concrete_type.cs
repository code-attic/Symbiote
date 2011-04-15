using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Core.Tests.DI.Container
{
    public class OpenConcrete<T>
    {
        public IMessageProvider Provider { get; set; }

        public OpenConcrete( IMessageProvider provider )
        {
            Provider = provider;
        }
    }

    public class PlainConcrete
    {
        public IMessageProvider Provider { get; set; }

        public PlainConcrete( IMessageProvider provider )
        {
            Provider = provider;
        }
    }

    public class when_requesting_concrete_type : with_simple_registration
    {
        private static string message;
        private Because of = () =>
            {
                var instance = Container.GetInstance<PlainConcrete>();
                message = instance.Provider.GetMessage();
            };

        private It should_have_correct_message = () => message.ShouldEqual( "This is a message from MessageHazzer. Hi!" );
    }

    public class when_requesting_concrete__open_type : with_simple_registration
    {
        private static string message;
        private Because of = () =>
            {
                var instance = Container.GetInstance<OpenConcrete<string>>();
                message = instance.Provider.GetMessage();
            };

        private It should_have_correct_message = () => message.ShouldEqual( "This is a message from MessageHazzer. Hi!" );
    }
}
