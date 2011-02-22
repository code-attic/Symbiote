namespace Core.Tests.Actor
{
    public class DummyActor
    {
        public string Id { get; set; }

        public DummyActor()
        {
            Instantiated++;
        }

        public static int Instantiated { get; set; }
    }
}