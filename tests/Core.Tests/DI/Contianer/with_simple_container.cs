using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.DI.Impl;
using Symbiote.Core.Extensions;

namespace Core.Tests.DI.Contianer
{
    public interface IHazzaMessage
    {
        string GetMessage();
    }

    public interface IMessageProvider
    {
        string GetMessage();
    }

    public class SimpleConstructor :
        IHazzaMessage
    {
        public string GetMessage()
        {
            return "Oh hai! This is simple constructor!";
        }
    }

    public class ComplexConstructor :
        IMessageProvider
    {
        public IHazzaMessage MessageHazzer { get; set; }

        public string GetMessage()
        {
            return MessageHazzer.GetMessage();
        }

        public ComplexConstructor( IHazzaMessage messageHazzer )
        {
            MessageHazzer = messageHazzer;
        }
    }

    class when_instantiating_without_container
    {
        static Stopwatch watch;
        private Because of = () => 
        {
            watch = Stopwatch.StartNew();

            Enumerable
                .Range( 0, 5000 )
                .ForEach( x => 
                {
                    var i = new ComplexConstructor( new SimpleConstructor() );
                });

            watch.Stop();
        };

        private It should_take_less_than_10_ms = () => watch.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 10 );
    }
    
    public 

    public interface IDependencyFactory
    {
        T Instantiate<T>();
        object Instantiate( Type type );
    }

    public class FactoryProvider
    {
        public IDependencyFactory CreateFactoryFor<T>()
        {

        }
    }

    public class when_getting_factory_for_simple_type
    {
        private Because of = () => 
        {

        };
    }

    public class when_instantiating_simple_type
    {

    }

    class with_simple_container
    {
        public static IDependencyFactory Factory { get; set; }

        Establish context = () =>
                                {

                                };
    }



    class when_instantiating_with_factories
        : with_simple_container
    {
        static Stopwatch watch;
        private Because of = () => 
        { 
            watch = Stopwatch.StartNew();

            Enumerable
                .Range( 0, 5000 )
                .ForEach( x => 
                {
                    
                });

            watch.Stop();
        };

        private It should_take_less_than_10_ms = () => watch.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 0 );
    }
}
