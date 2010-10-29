using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Transform;
using Symbiote.StructureMap;

namespace Messaging.Tests.Pipes
{
    public interface IPipe<TIn, TOut>
    {
        TOut Process(TIn input);
    }

    public interface IPipeline<TIn, TOut>
        : IPipe<TIn, TOut>
    {
        IPipeline<TIn, TNewOut> Then<TPipe, TNewOut>()
            where TPipe : IPipe<TOut, TNewOut>;

        IPipeline<TIn, TNewOut> Then<TNewOut>( IPipe<TOut, TNewOut> pipe );
    }

    public class Pipeline
    {
        public static IPipeline<TIn, TOut> New<TPipe, TIn, TOut>()
            where TPipe : IPipe<TIn, TOut>
        {
            return new Pipeline<TIn, TOut>(
                Assimilate.GetInstanceOf<TPipe>()
            );
        }    
    }

    public class Pipeline<TIn, TOut>
        : IPipeline<TIn, TOut>
    {
        protected IPipe<TIn, TOut> Pipe { get; set; }

        public IPipeline<TIn, TNewOut> Then<TPipe, TNewOut>() where TPipe : IPipe<TOut, TNewOut>
        {
            return new Pipeline<TIn, TNewOut>(
                Pipe.Combine<TPipe, TIn, TOut, TOut, TNewOut>()
            );
        }

        public IPipeline<TIn, TNewOut> Then<TNewOut>(IPipe<TOut, TNewOut> pipe)
        {
            return new Pipeline<TIn, TNewOut>( Pipe.Combine<TIn, TOut, TOut, TNewOut>( pipe ) );
        }


        public TOut Process( TIn input )
        {
            return Pipe.Process( input );
        }

        public Pipeline( IPipe<TIn, TOut> pipe )
        {
            Pipe = pipe;
        }
    }

    public class FuncPipe<TIn, TOut>
        : IPipe<TIn, TOut>
    {
        protected Func<TIn, TOut> Call { get; set; }

        public TOut Process( TIn input )
        {
            return Call( input );
        }

        public FuncPipe(Func<TIn, TOut> call)
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
            return new FuncPipe<X1, Y2>(x => pipe2.Process(begin.Process(x)));
        }

        public static IPipe<X1, Y2> Combine<X1, X2, Y1, Y2>(this IPipe<X1, X2> begin, IPipe<X2, Y2> end)
            where X2 : Y1
        {
            return new FuncPipe<X1, Y2>(x => end.Process(begin.Process(x)));
        }

        public static IPipeline<TIn, TOut> WireUp<TIn, TOut, TMid>( this IPipe<TMid, TOut> pipe, IPipeline<TIn, TMid> heads )
        {
            return heads.Then( pipe );
        }

        public static IPipeline<TIn, TOut> WireUp<TIn, TOut, TMid>( this IPipe<TMid, TOut> tails, IPipe<TIn, TMid> heads )
        {
            return new Pipeline<TIn, TOut>( heads.Combine<TIn, TMid, TMid, TOut>( tails ) );
        }

        public static B Pipe<TPipe, A, B>(this A value, TPipe pipe) 
            where TPipe : IPipe<A, B>
        {
            return pipe.Process( value );
        }

    }

    public abstract class Pipe<TIn, TOut>
        : IPipe<TIn, TOut>
    {
        public abstract TOut Process( TIn input );

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

    public class TransformIntToString
        : BaseTransform<int, string>
    {
        public override string Transform( int origin )
        {
            return origin.ToString();
        }

        public override int Reverse( string transformed )
        {
            return int.Parse( transformed );
        }
    }

    public class TransformStringToBytes
        : BaseTransform<string, byte[]>
    {
        public override byte[] Transform( string origin )
        {
            return Encoding.UTF8.GetBytes( origin );
        }

        public override string Reverse( byte[] transformed )
        {
            return Encoding.UTF8.GetString( transformed );
        }
    }

    public class when_combining_delegates
    {
        protected static int test1;
        protected static int test2;
        protected static int test3;

        private Establish context = () =>
        {
            Assimilate.Core<StructureMapAdapter>();
        };

        private Because of = () =>
        {
            test1 = Pipeline
                .New<IntToString, int, string>()
                .Then(new StringToBytes())
                .Then<BytesToString, string>()
                .Then<StringToInt, int>()
                .Process( 10 );

            test2 = new StringToInt()
                .WireUp( new BytesToString() )
                .WireUp( new StringToBytes() )
                .WireUp( new IntToString() )
                .Process( 10 );

            var transformer = new Transformer()
                .Then<TransformIntToString>()
                .Then<TransformStringToBytes>();

            var temp = transformer.Transform<int, byte[]>( 10 );
            test3 = transformer.Reverse<byte[], int>( temp );
        };

        private It should_get_10_for_test1 = () => test1.ShouldEqual( 10 );
        private It should_get_10_for_test2 = () => test2.ShouldEqual( 10 );
        private It should_get_10_for_test3 = () => test3.ShouldEqual( 10 );
    }
}
