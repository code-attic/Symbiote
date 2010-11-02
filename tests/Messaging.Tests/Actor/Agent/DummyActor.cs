namespace Messaging.Tests.Actor.Agent
{
    public class DummyActor
    {
        public static int Instantiated { get; set; }

        public DummyActor()
        {
            Instantiated++;
        }
    }
}