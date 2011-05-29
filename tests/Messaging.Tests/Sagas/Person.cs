using System;

namespace Actor.Tests.Sagas
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Initialized { get; set; }

        public Person()
        {
            Id = Guid.Empty;
        }
    }
}