namespace Core.Tests.Json.Decorator
{
    public class Underlying
        : IHaveKey
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}