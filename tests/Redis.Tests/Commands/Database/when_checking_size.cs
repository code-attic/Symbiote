using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Redis.Tests.Commands.Database
{
    public class when_checking_size :
        with_clean_db
    {
        protected static int dbSize;

        private Because of = () =>
        {
            dbSize = 0;
            client.Set("DbSize Key", 0);
            dbSize = client.DbSize;
        };

        private It should_have_a_correct_size = () => dbSize.ShouldEqual(1);
    }
}