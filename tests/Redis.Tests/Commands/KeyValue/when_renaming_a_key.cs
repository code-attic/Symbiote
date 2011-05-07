using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Redis.Tests.Commands.KeyValue
{
    class when_renaming_a_key :
        with_clean_db
    {
        protected static bool oldKeyExists;
        protected static bool renameRsltSame;
        protected static bool renameRsltDiff;
        protected static int valDiff;

        private Because of = () =>
        {
            var key1 = "Int Rename Key";
            var key2 = "New Rename Key";
            client.Set(key1, 1);
            client.Set(key2, 2);
            renameRsltSame = client.Rename(key2, key2);
            renameRsltDiff = client.Rename(key2, key1);
            valDiff = client.Get<int>(key1);
            oldKeyExists = client.Exists(key2);
        };

        private It should_fail_when_renaming_to_same_name = () => renameRsltSame.ShouldBeFalse();
        private It should_succeed_when_renaming_to_different_name = () => renameRsltDiff.ShouldBeTrue();
        private It should_no_longer_have_old_key_once_renamed = () => oldKeyExists.ShouldBeFalse();
        private It should_overwrite_an_existing_key_of_same_name = () => valDiff.ShouldEqual(2);

    }
}
