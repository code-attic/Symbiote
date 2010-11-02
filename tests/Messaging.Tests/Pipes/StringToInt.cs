namespace Messaging.Tests.Pipes
{
    public class StringToInt
        : IPipe<string, int>
    {
        public int Process( string input )
        {
            return int.Parse( input );
        }
    }
}