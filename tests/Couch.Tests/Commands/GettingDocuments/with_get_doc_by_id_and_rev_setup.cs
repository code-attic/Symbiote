using Machine.Specifications;

namespace Couch.Tests.Commands.GettingDocuments
{
    public abstract class with_get_doc_by_id_and_rev_setup : with_get_doc_by_id_setup
    {
        private Establish context = () =>
                                        {
                                            url = @"http://localhost:5984/symbiotecouch/1?rev=1";
                                        };
    }
}