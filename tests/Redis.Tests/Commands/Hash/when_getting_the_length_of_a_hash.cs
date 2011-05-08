using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Redis.Tests.Commands.Hash
{
    public class when_getting_the_length_of_a_hash :
        with_clean_db
    {
        protected static bool LengthCorrect;
        protected static bool LengthCorrectForEmptyHash;

        private Because of = () =>
        {
            var key = "Test Hash Key Len";
            var field = "Test Hash Field Len {0}";
            LengthCorrectForEmptyHash = client.HLen(key) == 0;
            for (int i = 0; i < 5; i++)
            {
                client.HSet(key, field.AsFormat(i), i);
            }
            LengthCorrect = client.HLen(key) == 5;
        };

        private It should_get_zero_lenght_for_nonexistant_hash = () => LengthCorrectForEmptyHash.ShouldBeTrue();
        private It should_get_correct_length_the_hash = () => LengthCorrect.ShouldBeTrue();

    }
}
