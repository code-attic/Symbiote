using Machine.Specifications;
using Symbiote.Core;
using Symbiote.StructureMapAdapter;

namespace Couch.Tests.Document
{
    public class with_custom_document_complex_key : with_custom_document_simple_key
    {
        protected static Driver driver;

        private Establish context = () =>
                                        {
                                            Assimilate.Initialize();
                                            driver = new Driver(person);
                                        };
    }
}