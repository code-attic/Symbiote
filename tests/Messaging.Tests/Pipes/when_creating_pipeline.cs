using System.Collections.Generic;
using System.Linq;
using Symbiote.Core.Extensions;
using Symbiote.Messaging;

namespace Messaging.Tests.Pipes
{
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
}
