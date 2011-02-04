using System;
using System.Linq;
using Machine.Specifications;
using Moq;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Messaging.Impl.Channels.Local;
using It = Machine.Specifications.It;

namespace Messaging.Tests.Channels.Manager
{
    public class TestChannelDefinition
        : IChannelDefinition
    {
        public string Name { get; set; }
        public Type ChannelType { get; set; }
        public Type MessageType { get; set; }
        public Type FactoryType { get; set; }
        public Type SerializerType { get; set; }

        public TestChannelDefinition( string name, Type messageType )
        {
            Name = name;
            MessageType = messageType;
            ChannelType = typeof(TestChannelDefinition);
            FactoryType = typeof(LocalChannelFactory);
        }
    }

    public class DummyMessage
    {
        
    }

    public class with_channel_index
        : with_assimilation
    {
        protected static ChannelIndex Index { get; set; }
        protected static TestChannelDefinition ChannelDef1 { get; set; }
        protected static TestChannelDefinition ChannelDef2 { get; set; }
        protected static TestChannelDefinition ChannelDef3 { get; set; }
        protected static TestChannelDefinition ChannelDef4 { get; set; }

        private Establish context = () =>
        {
            ChannelDef1 = new TestChannelDefinition("test1", typeof(DummyMessage));
            ChannelDef2 = new TestChannelDefinition("test2", typeof(DummyMessage));
            ChannelDef3 = new TestChannelDefinition("test3", typeof(DummyMessage));
            ChannelDef4 = new TestChannelDefinition("test4", typeof(DummyMessage));
            Index = new ChannelIndex();
        };
    }

    public class with_mock_index
        : with_assimilation
    {
        protected static IChannelIndex Index { get { return MockIndex.Object; } }
        protected static Mock<IChannelIndex> MockIndex { get; set; }

        private Establish context = () =>
        {
            MockIndex = new Mock<IChannelIndex>();
        };
    }

    public class with_channel_manager
        : with_mock_index
    {
        protected static ChannelManager Manager { get; set; }
        
        private Establish context = () =>
        {
            Manager = new ChannelManager(Index);
        };
    }

    public class when_adding_definition
        : with_channel_index
    {
        protected static Exception Exception { get; set; }
        private Because of = () =>
        {
            Exception = Catch.Exception(() => 
                Index.AddDefinition( ChannelDef1 ));
        };

        private It should_have_channel_by_name = () =>
            Index.HasChannelFor("test1");

        private It should_not_cause_exception = () => 
            Exception.ShouldBeNull();
    }

    public class when_adding_invalid_channel_definition
        : with_channel_index
    {
        protected static InvalidChannelDefinitionException Exception { get; set; }
        protected static TestChannelDefinition InvalidChannelDef { get; set; }
        private Because of = () =>
        {
            InvalidChannelDef = new TestChannelDefinition( "invalid", typeof(DummyMessage) );
            InvalidChannelDef.FactoryType = null;
            InvalidChannelDef.ChannelType = null;
            InvalidChannelDef.MessageType = null;
            InvalidChannelDef.Name = null;

            Exception = Catch.Exception( () =>
                Index.AddDefinition( InvalidChannelDef ) ) as InvalidChannelDefinitionException;
        };

        private It should_throw_invalid_channel_exception = () => 
            Exception.ShouldBeOfType<InvalidChannelDefinitionException>();

        private It should_have_proper_warnings_for_each_violation = () =>
            Exception.Violations.ShouldContain(
                    "Channel definition must specify a channel factory type",
                    "Channel definition must specify a channel type",
                    "Channel definition must specify a channel name"
                );
    }

    public class when_adding_duplicate_channel_definition
        : with_channel_manager
    {
        
    }

    public class when_adding_definitions_in_parallel
        : with_channel_index
    {
        private Because of = () =>
        {
            var defList = new[]
            {
                ChannelDef1,
                ChannelDef2,
                ChannelDef3,
                ChannelDef4
            };

            defList.AsParallel()
                .ForAll( x => Index.AddDefinition( x ) );
        };

        private It should_have_definition_1_by_name = () =>
            Index.HasChannelFor("test1");
        private It should_have_definition_2_by_name = () =>
            Index.HasChannelFor("test2");
        private It should_have_definition_3_by_name = () =>
            Index.HasChannelFor("test3");
        private It should_have_definition_4_by_name = () =>
            Index.HasChannelFor("test4");
        private It should_have_definitions_for_4 = () =>
            Index.Definitions.Count.ShouldEqual(4);
        private It should_have_4_channels_for_type = () =>
            Index.Definitions.Count.ShouldEqual(4);
    }

    public class when_requesting_missing_definition
        : with_channel_index
    {
        protected static MissingChannelDefinitionException Exception { get; set; }
        private Because of = () =>
        {
            Exception = Catch.Exception( () => 
                Index.GetDefinition("test1")) as MissingChannelDefinitionException;
        };

        private It should_cause_exception = () =>
            Exception.Message.ShouldEqual("There was no definition provided for a channel named test of message type Messaging.Tests.Channels.Manager.DummyMessage. Please check that you have defined a channel before attempting to use it.");
    }

    public class when_getting_channel_list_for_message
        : with_channel_manager
    {
        private Because of = () =>
        {
            
        };
    }

    public class when_getting_channel_by_name
        : with_channel_manager
    {
        protected static IChannel FirstRequest { get; set; }
        protected static IChannel SecondRequest { get; set; }

        private Because of = () =>
        {
            MockIndex
                .Setup( x => x.GetDefinition( "test" ) )
                .Returns( new LocalChannelDefinition() {} );

            FirstRequest = Manager.GetChannelFor("test");
            SecondRequest = Manager.GetChannelFor("test");
        };
        
        private It should_have_asked_for_definition_once = () => 
            MockIndex.Verify( x => x.GetDefinition( "test" ), Times.Once());

        private It should_instantiate_valid_channel = () => 
            FirstRequest.ShouldNotBeNull();

        private It should_have_returned_same_channel_on_second_request = () => 
            SecondRequest.ShouldEqual( FirstRequest );
    }
}
