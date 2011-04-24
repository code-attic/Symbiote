namespace Mikado.Tests.Domain.Model
{
    public class Manager : Person, IManager
    {
        public string Department { get; set; }
    }
}