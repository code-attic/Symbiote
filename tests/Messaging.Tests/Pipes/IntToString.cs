namespace Messaging.Tests.Pipes
{
    public class IntToString
        : IPipe<int, string>
    {
        public string Process( int input )
        {
            return input.ToString();
        }
    }
}