using System.Threading;
using Machine.Specifications;

namespace Messaging.Tests.Local.HandleInterface
{
    public class when_testing_handling_messages_with_common_interface
        : with_bus
    {
        private Because of = () =>
                                 {
                                     bus.Publish( "local", new MessageA() { Text = "A" } );
                                     bus.Publish( "local", new MessageB() { Text = "B" } );
                                     Thread.Sleep( 3 );
                                 };

        private It should_have_both_messages = () => 
            HandleMessages.Messages.Count.ShouldEqual( 2 );
    }
}