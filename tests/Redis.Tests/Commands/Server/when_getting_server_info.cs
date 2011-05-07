using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Redis.Tests.Commands.Server
{
    class when_getting_server_info :
        with_clean_db
    {
        protected static Dictionary<string, string> infoResults;

        private Because of = () =>
        {
            infoResults = client.GetInfo();
        };

        private It should_get_a_list_of_data = () => infoResults.Count.ShouldBeGreaterThan(0);

    }
}
