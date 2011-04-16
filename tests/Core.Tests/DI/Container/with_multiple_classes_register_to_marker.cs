using Machine.Specifications;
using Symbiote.Core.DI;

namespace Core.Tests.DI.Container
{
    public class with_multiple_classes_register_to_marker : with_container
    {
        private Establish context = () =>
            {
                var def1 = DependencyExpression.For<IMark>();
                def1.Add<Multiple1>();

                var def2 = DependencyExpression.For<IMark>();
                def2.Add<Multiple2>();

                var def3 = DependencyExpression.For<IMark>();
                def3.Add<Multiple3>();

                Container.Register( def1 );
                Container.Register( def2 );
                Container.Register( def3 );
            };
    }
}