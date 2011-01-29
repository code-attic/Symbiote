namespace Actor.Tests
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