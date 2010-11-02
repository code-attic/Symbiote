using Symbiote.Core;

namespace Messaging.Tests.Pipes
{
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
}