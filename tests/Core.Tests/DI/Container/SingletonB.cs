namespace Core.Tests.DI.Container
{
    public class SingletonB : IShouldBeSingleton
    {
        private int _instance;
        static int Instantiated { get; set; }

        public int Instance
        {
            get { return _instance; }
        }

        public SingletonB()
        {
            _instance = Instantiated ++;
        }
    }
}