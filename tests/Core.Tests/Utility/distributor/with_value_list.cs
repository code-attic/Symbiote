using System.Collections.Generic;
using Machine.Specifications;

namespace Core.Tests.Utility
{
    public abstract class with_value_list
    {
        protected static List<string> wordList;
        private Establish context = () =>
                                        {
                                            wordList = new List<string>()
                                                           {
                                                               "dog",
                                                               "dingo",
                                                               "elephant",
                                                               "emu",
                                                               "fargo",
                                                               "fox",
                                                               "jackolantern",
                                                               //"apple",
                                                               //"ardvark",
                                                               //"bear",
                                                               //"biscuit",
                                                               //"card",
                                                               //"cracker",
                                                               //"gopher",
                                                               //"grotto",
                                                               //"great",
                                                               //"hangar",
                                                               //"octopus",
                                                               //"pirate",
                                                               //"queen",
                                                               //"rhinocerous",
                                                               //"stereo",
                                                               //"tricycle",
                                                               //"umbrella",
                                                               //"van",
                                                               //"wing",
                                                               //"xylophone",
                                                               //"zebra",
                                                               //"ice",
                                                               //"jar",
                                                               //"kite",
                                                               //"leopard",
                                                               //"microphone",
                                                               //"neck",
                                                               //"zoo",
                                                               //"bag",
                                                               //"cat",
                                                               //"hippopotamus",
                                                               //"mower",
                                                           };
                                        };
    }
}