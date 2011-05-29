using System;

namespace Actor.Tests.Sagas
{
    public class SetPersonName
    {
        public Guid PersonId { get; set; }
        public string Name { get; set; }

        public SetPersonName()
        {
            PersonId = Guid.Empty;
        }
    }
}