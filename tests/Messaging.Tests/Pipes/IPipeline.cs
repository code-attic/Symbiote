namespace Messaging.Tests.Pipes
{
    public interface IPipeline<TIn, TOut>
        : IPipe<TIn, TOut>
    {
        IPipeline<TIn, TNewOut> Then<TPipe, TNewOut>()
            where TPipe : IPipe<TOut, TNewOut>;

        IPipeline<TIn, TNewOut> Then<TNewOut>( IPipe<TOut, TNewOut> pipe );
    }
}