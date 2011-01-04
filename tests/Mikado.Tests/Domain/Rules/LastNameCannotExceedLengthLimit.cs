using Mikado.Tests.Domain.Model;
using Symbiote.Mikado.Impl;

namespace Mikado.Tests.Domain.Rules
{
    public class LastNameCannotExceedLengthLimit : Rule<Person>
    {
        public LastNameCannotExceedLengthLimit()
            : base(x => x.LastName.Length <= 25, "LastName cannot exceed 25 characters.")
        {

        }
    }
}