using Machine.Specifications;
using Mikado.Tests.Domain.Model;

namespace Mikado.Tests.TestSetup
{
    public abstract class with_Person : with_ioc_configuration
    {
        public static Person Person;

        private Establish context = () =>
                                        {
                                            Person = new Person()
                                                         {
                                                             Age = 37,
                                                             FirstName = "Jim",
                                                             LastName = "Cowart"
                                                         };
                                        };
    }
}