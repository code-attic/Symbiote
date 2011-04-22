namespace Core.Tests.DI.Container
{
    public class SingletonA : IShouldBeSingleton
    {
        private int _instance;
        static int Instantiated { get; set; }

        public static void Reset()
        {
            Instantiated = 0;
        }

        public int Instance
        {
            get { return _instance; }
        }

        public string Message { get; set; }

        public SingletonA()
        {
            _instance = Instantiated ++;
        }
    }
}