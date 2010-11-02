namespace Messaging.Tests.Pipes
{
    public interface IPipe<TIn, TOut>
    {
        TOut Process(TIn input);
    }
}