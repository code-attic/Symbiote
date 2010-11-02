using Symbiote.Core;

namespace Messaging.Tests.Pipes
{
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
}