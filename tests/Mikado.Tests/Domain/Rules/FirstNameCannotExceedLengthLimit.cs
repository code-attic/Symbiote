using System;
using Mikado.Tests.Domain.Model;
using Symbiote.Mikado.Impl;

namespace Mikado.Tests.Domain.Rules
{
    public class FirstNameCannotExceedLengthLimit : Rule<Person>
    {
        public FirstNameCannotExceedLengthLimit( ICustomerService service )
            :base(x => x.FirstName.Length <= service.GetNameLengthLimit( x ), s => String.Format("FirstName cannot exceed {0} characters.", service.GetNameLengthLimit( s )))
        {
            
        }
    }

    public interface ICustomerService
    {
        int GetNameLengthLimit( IHaveFirstName instance );
    }

    public class CustomerService : ICustomerService
    {
        public int GetNameLengthLimit( IHaveFirstName instance )
        {
            if( instance is Manager )
            {
                return 25;
            }
            return 20;
        }
    }
}