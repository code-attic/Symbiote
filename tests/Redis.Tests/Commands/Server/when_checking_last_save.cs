using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Redis.Tests.Commands.Server
{
    public class when_checking_last_save :
        with_clean_db
    {
        protected static DateTime saveDate;

        private Because of = () =>
        {
            saveDate = client.LastSave;
        };

        private It should_get_a_valid_save_date = () => saveDate.ShouldNotBeNull();

    }
}
