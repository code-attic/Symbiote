namespace Mikado.Tests.Domain.Model
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Ssn { get; set; }
        public int Age { get; set; }
    }

    public class Manager : Person
    {
        public string Department { get; set; }
    }
}
