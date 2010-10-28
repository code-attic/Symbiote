using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.StructureMap;

namespace Messaging.Tests.Pipes
{
    public interface IPipeline<TIn, TOut>
    {
        IPipe<TIn, TOut> Pipe { get; set; }

        IPipeline<TIn, TNewOut> Then<TPipe, TNewOut>()
            where TPipe : IPipe<TOut, TNewOut>;
    }

    public class Pipeline<TIn, TOut>
        : IPipeline<TIn, TOut>
    {
        public IPipe<TIn, TOut> Pipe { get; set; }

        public IPipeline<TIn, TNewOut> Then<TPipe, TNewOut>() where TPipe : IPipe<TOut, TNewOut>
        {
            return new Pipeline<TIn, TNewOut>()
            {
                Pipe = Pipe.Combine<TPipe, TIn, TOut, TOut, TNewOut>()
            };
        }

        public static IPipeline<TIn, TOut> New<TPipe>()
            where TPipe : IPipe<TIn, TOut>
        {
            return new Pipeline<TIn, TOut>()
            {
                Pipe = Assimilate.GetInstanceOf<TPipe>()
            };
        }
    }

    public interface IPipe<TIn, TOut>
    {
        TOut Process( TIn input );
    }

    public class Fitting<TIn, TOut>
        : IPipe<TIn, TOut>
    {
        protected Func<TIn, TOut> Call { get; set; }

        public TOut Process( TIn input )
        {
            return Call( input );
        }

        public Fitting( Func<TIn, TOut> call )
        {
            Call = call;
        }
    }

    public static class PipelineExtensions
    {
        public static IPipe<X1, Y2> Combine<P2, X1, X2, Y1, Y2>(this IPipe<X1, X2> begin)
            where P2 : IPipe<Y1, Y2>
            where X2 : Y1
        {
            var pipe2 = Assimilate.GetInstanceOf<P2>();
            return new Fitting<X1, Y2>(x => pipe2.Process(begin.Process(x)));
        }

        public static Func<X1, Y2> Collapse<X1, X2, Y1, Y2>(this Func<X1, X2> func1, Func<Y1, Y2> func2)
            where X2 : Y1
        {
            Func<X1, Y2> combined = input => func2(func1(input));
            return combined;
        }
    }

    public class IntToString
        : IPipe<int, string>
    {
        public string Process( int input )
        {
            return input.ToString();
        }
    }

    public class StringToBytes
        : IPipe<string, byte[]>
    {
        public byte[] Process( string input )
        {
            return Encoding.UTF8.GetBytes( input );
        }
    }

    public class BytesToString
        : IPipe<byte[], string>
    {
        public string Process( byte[] input )
        {
            return Encoding.UTF8.GetString( input );
        }
    }

    public class StringToInt
        : IPipe<string, int>
    {
        public int Process( string input )
        {
            return int.Parse( input );
        }
    }

    public class when_combining_delegates
    {
        protected static int test;

        private Establish context = () =>
        {
            Assimilate.Core<StructureMapAdapter>();
        };

        private Because of = () =>
        {
            test = Pipeline<int, string>
                .New<IntToString>()
                .Then<StringToBytes, byte[]>()
                .Then<BytesToString, string>()
                .Then<StringToInt, int>()
                .Pipe.Process( 10 );
        };

        private It should_get_10 = () => test.ShouldEqual( 10 );
    }
}
