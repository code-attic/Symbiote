using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Messaging.Impl.Transform;

namespace Messaging.Tests.Channels.Manager
{
    public class TestChannelDefinition
        : IChannelDefinition
    {
        public string Name { get; set; }
        public Type ChannelType { get; set; }
        public Type MessageType { get; set; }
        public Type FactoryType { get; set; }
        public Transformer OutgoingTransform { get; set; }
        public Transformer IncomingTransform { get; set; }

        public TestChannelDefinition( string name, Type messageType )
        {
            Name = name;
            MessageType = messageType;
            ChannelType = typeof(TestChannelDefinition);
            FactoryType = typeof(LocalChannelFactory<DummyMessage>);
        }
    }

    public class DummyMessage
    {
        
    }

    public class with_channel_manager
        : with_assimilation
    {
        protected static ChannelManager Manager { get; set; }

        protected static TestChannelDefinition ChannelDef1 { get; set; }
        protected static TestChannelDefinition ChannelDef2 { get; set; }
        protected static TestChannelDefinition ChannelDef3 { get; set; }
        protected static TestChannelDefinition ChannelDef4 { get; set; }
        
        private Establish context = () =>
        {
            Manager = new ChannelManager();

            ChannelDef1 = new TestChannelDefinition( "test1", typeof(DummyMessage) );
            ChannelDef2 = new TestChannelDefinition( "test2", typeof(DummyMessage) );
            ChannelDef3 = new TestChannelDefinition( "test3", typeof(DummyMessage) );
            ChannelDef4 = new TestChannelDefinition( "test4", typeof(DummyMessage) );
        };
    }

    public class when_adding_definition
        : with_channel_manager
    {
        protected static Exception Exception { get; set; }
        private Because of = () =>
        {
            Exception = Catch.Exception(() => 
                Manager.AddDefinition( ChannelDef1 ));
        };

        private It should_have_channel_for_message = () => 
            Manager.HasChannelFor<DummyMessage>();

        private It should_have_channel_by_name = () => 
            Manager.HasChannelFor<DummyMessage>("test1");

        private It should_not_cause_exception = () => 
            Exception.ShouldBeNull();
    }

    public class when_adding_invalid_channel_definition
        : with_channel_manager
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
                Manager.AddDefinition( InvalidChannelDef ) ) as InvalidChannelDefinitionException;
        };

        private It should_throw_invalid_channel_exception = () => 
            Exception.ShouldBeOfType<InvalidChannelDefinitionException>();

        private It should_have_proper_warnings_for_each_violation = () =>
            Exception.Violations.ShouldContain(
                    "Channel definition must specify a channel factory type",
                    "Channel definition must specify a channel type",
                    "Channel definition must specify a message type",
                    "Channel definition must specify a channel name"
                );
    }

    public class when_adding_duplicate_channel_definition
        : with_channel_manager
    {
        
    }

    public class when_adding_definitions_in_parallel
        : with_channel_manager
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
                .ForAll( x => Manager.AddDefinition( x ) );
        };

        private It should_have_definition_1_by_name = () => 
            Manager.HasChannelFor<DummyMessage>("test1");
        private It should_have_definition_2_by_name = () => 
            Manager.HasChannelFor<DummyMessage>("test2");
        private It should_have_definition_3_by_name = () => 
            Manager.HasChannelFor<DummyMessage>("test3");
        private It should_have_definition_4_by_name = () =>
            Manager.HasChannelFor<DummyMessage>("test4");
        private It should_have_definitions_for_4 = () => 
            Manager.Definitions.Count.ShouldEqual( 4 );
        private It should_have_4_channels_for_type = () => 
            Manager.MessageChannels[typeof(DummyMessage)].Count.ShouldEqual( 4 );
        private It should_have_1_channel_factory = () => 
            Manager.ChannelFactories.Count.ShouldEqual( 1 );
        private It should_not_instantiate_channels = () => 
            Manager.Channels.Count.ShouldEqual( 0 );
    }

    public class when_requesting_missing_channel
        : with_channel_manager
    {
        protected static MissingChannelDefinitionException Exception { get; set; }
        private Because of = () =>
        {
            Exception = Catch.Exception( () => 
                Manager.GetChannelFor<DummyMessage>()) as MissingChannelDefinitionException;
        };

        private It should_cause_exception = () =>
            Exception.Message.ShouldEqual("There was no definition provided for a channel named <nothing> of message type DummyMessage. Please check that you have defined a channel before attempting to use it.");
    }

    public class when_getting_channel_list_for_message
        : with_channel_manager
    {
        
    }

    public class when_getting_channel_by_name
        : with_channel_manager
    {
        
    }
}
