using Machine.Specifications;
using Symbiote.Couch.Impl.Model;

namespace Couch.Tests.Document
{
    public abstract class with_design_document
    {
        public static DesignDocument doc;

        private Establish context = () =>
                                        {
                                            doc = new DesignDocument()
                                                      {
                                                          DocumentId = @"design/test",
                                                          Views =
                                                              {
                                                                  {"one", new DesignView() { Map = @"function(doc) { emit(doc); }"}}
                                                              }
                                                      };
                                        };
    }
}