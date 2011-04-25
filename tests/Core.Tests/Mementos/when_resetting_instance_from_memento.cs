using Machine.Specifications;

namespace Core.Tests.Mementos
{
    public class when_resetting_instance_from_memento : with_driver
    {
        private Because of = () => 
                                 { 
                                     var memento = Memoizer.GetMemento( driver );
                                     driver.ChangeName( "Dumb", "Face" );
                                     Memoizer.ResetToMemento( driver, memento );
                                 };

        private It should_have_original_first_name = () => driver.FirstName.ShouldEqual( "Mr" );
        private It should_have_original_last_name = () => driver.LastName.ShouldEqual( "Rogers" );
    }
}