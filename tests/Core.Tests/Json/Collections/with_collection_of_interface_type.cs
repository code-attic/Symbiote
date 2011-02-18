using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Core.Tests.Json.Collections
{
    public abstract class with_collection_of_interface_type
    {
        protected static List<IGotsaNameYo> collection;
        protected static Stuff stuff;

        private Establish context = () =>
                                        {
                                            collection = new List<IGotsaNameYo>()
                                                             {
                                                                 new Car() {Make="Chevy",Name="Equinox",Year=2007},
                                                                 new Person() {Age=30, Name="Me"}
                                                             };

                                            stuff = new Stuff()
                                                        {
                                                            list = collection,
                                                            Thingy = new Car() { Make="Honda",Name="Civic",Year=2009}
                                                        };
                                        };
    }
}
