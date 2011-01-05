using Mikado.Tests.Domain.Model;
using Symbiote.Mikado.Impl;

namespace Mikado.Tests.Domain.Rules
{
    public class FirstNameCannotExceedLengthLimitRule : Rule<Person>
    {
        public FirstNameCannotExceedLengthLimitRule( ICustomerService service )
            :base(x => x.FirstName.Length <= service.GetNameLengthLimit( x ), "FirstName cannot exceed 20 characters.")
        {
            
        }
    }

    public interface ICustomerService
    {
        int GetNameLengthLimit( Person person );
    }

    public class CustomerService : ICustomerService
    {
        public int GetNameLengthLimit( Person person )
        {
            if( person is Manager )
            {
                return 25;
            }
            return 20;
        }
    }
}