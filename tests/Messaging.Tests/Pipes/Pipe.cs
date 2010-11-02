namespace Messaging.Tests.Pipes
{
    public abstract class Pipe<TIn, TOut>
        : IPipe<TIn, TOut>
    {
        public abstract TOut Process( TIn input );

    }
}