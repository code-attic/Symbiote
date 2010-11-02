using System;

namespace Messaging.Tests.Pipes
{
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
}