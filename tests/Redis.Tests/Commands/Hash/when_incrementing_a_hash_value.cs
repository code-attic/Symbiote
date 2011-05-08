using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Redis.Tests.Commands.Hash
{
    public class when_incrementing_a_hash_value :
        with_clean_db
    {
        protected static bool ValIncrByOne;
        protected static bool ValIncrRepeatedlyByOne;
        protected static bool ValIncrByFive;
        protected static bool RsltIncrByOne;
        protected static bool RsltIncrByFive;
        protected static bool ValIncrByNegOne;
        protected static bool ValIncrByNegFive;
        protected static bool RsltIncrByNegOne;
        protected static bool RsltIncrByNegFive;

        private Because of = () =>
        {
            var key = "Test Hash Key Incr";
            var field = "Test Hash Field Incr";
            client.HSet(key, field, 1);
            var rslt = client.HIncrementBy(key, field, 1);
            RsltIncrByOne = rslt == 2;
            ValIncrByOne = client.HGet<int>(key, field) == rslt;
            for( int i = 0; i < 4; i++ )
            {
                rslt = client.HIncrementBy(key, field, 1);
            }
            ValIncrRepeatedlyByOne = rslt == 6;
            rslt = client.HIncrementBy(key, field, 5);
            RsltIncrByFive = rslt == 11;
            ValIncrByFive = client.HGet<int>(key, field) == rslt;

            //Now do negative steps
            rslt = client.HIncrementBy(key, field, -1);
            RsltIncrByNegOne = rslt == 10;
            ValIncrByNegOne = client.HGet<int>(key, field) == rslt;
            rslt = client.HIncrementBy(key, field, -5);
            RsltIncrByNegFive = rslt == 5;
            ValIncrByNegFive = client.HGet<int>(key, field) == rslt;

            

        };

        private It should_increment_by_one = () => ValIncrByOne.ShouldBeTrue();
        private It should_return_the_correct_result_when_incrementing_by_one = () => RsltIncrByOne.ShouldBeTrue();
        private It should_increment_by_one_repeatedly = () => ValIncrRepeatedlyByOne.ShouldBeTrue();
        private It should_increment_by_greater_than_one = () => ValIncrByFive.ShouldBeTrue();
        private It should_return_the_correct_result_when_incrementing_by_greater_than_one = () => RsltIncrByFive.ShouldBeTrue();
        private It should_decrement_by_one = () => ValIncrByNegOne.ShouldBeTrue();
        private It should_return_the_correct_result_when_decrementing_by_one = () => RsltIncrByNegOne.ShouldBeTrue();
        private It should_decrement_by_greater_than_one = () => ValIncrByNegFive.ShouldBeTrue();
        private It should_return_the_correct_result_when_decrementing_by_greater_than_one = () => RsltIncrByNegFive.ShouldBeTrue();

    }
}
