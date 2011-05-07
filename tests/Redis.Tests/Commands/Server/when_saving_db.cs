using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Redis.Tests.Commands.Server
{
    class when_saving_db :
        with_clean_db
    {
        protected static bool SaveRslt;
        protected static bool AsyncSaveRslt;

        private Because of = () =>
        {
            SaveRslt = client.Save();
            AsyncSaveRslt = client.BackgroundSave();
        };

        private It should_save_the_db_synchronously = () => SaveRslt.ShouldBeTrue();
        private It should_save_the_db_asynchronously = () => AsyncSaveRslt.ShouldBeTrue();

    }
}
