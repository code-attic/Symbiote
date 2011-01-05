using Mikado.Tests.Domain.Model;
using Symbiote.Mikado.Impl;

namespace Mikado.Tests.Domain.Rules
{
    public class LastNameCannotExceedLengthLimitRule : Rule<Person>
    {
        public LastNameCannotExceedLengthLimitRule()
            : base(x => x.LastName.Length <= 25, "LastName cannot exceed 25 characters.")
        {

        }
    }
}