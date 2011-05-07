using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Redis.Tests.Commands.KeyValue
{
    public class when_setting_int_value:
        with_redis_client
    {
        protected static int initialVal;
        protected static string key;
        protected static int dbVal;
        protected static bool valInDb;

        private Because of = () =>
                                 {
                                     client.SelectDb(1);
                                     client.FlushDb();

                                     initialVal = 234;
                                     key = "Int Set Key";
                                     client.Set(key, initialVal);
                                     
                                     try 
	                                 {	        
		                                dbVal = client.Get<int>(key);
                                        valInDb = true;
	                                 }
	                                 catch (Exception e)
	                                 {
                                        valInDb = false;
	 	                                throw;
	                                 }

                                     client.FlushDb();
                                 };

        private It should_exist_in_the_db = () => 
            valInDb.ShouldBeTrue();
        private It should_equal_the_original_value = () => dbVal.ShouldEqual( initialVal);

    }
}
