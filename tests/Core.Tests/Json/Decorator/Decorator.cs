namespace Core.Tests.Json.Decorator
{
    public class Decorator<T>
    {
        public string Revision { get; set; }
        public T Base { get; set; }
    }
}