using Mikado.Tests.Domain.Model;
using Symbiote.Mikado.Impl;

namespace Mikado.Tests.Domain.Rules
{
    public class AgeMustBePositiveInteger : Rule<IHaveAge>
    {
        public AgeMustBePositiveInteger()
            : base(x => x.Age >= 0, "Age must be greater than or equal to 0.")
        {

        }
    }
}