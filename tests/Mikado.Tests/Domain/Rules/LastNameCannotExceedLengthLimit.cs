using Mikado.Tests.Domain.Model;
using Symbiote.Mikado.Impl;

namespace Mikado.Tests.Domain.Rules
{
    public class LastNameCannotExceedLengthLimit : Rule<IHaveLastName>
    {
        public LastNameCannotExceedLengthLimit()
            : base(x => x.LastName.Length <= 25, "LastName cannot exceed 25 characters.")
        {

        }
    }
}