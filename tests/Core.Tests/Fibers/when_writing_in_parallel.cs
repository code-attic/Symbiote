using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Core.Tests.Fibers
{
    public class when_writing_in_parallel
        : with_director
    {
        static List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        static List<string> targets = new List<string>() { "one", "two", "three" };

        private Because of = () =>
            {
                targets
                    .AsParallel()
                    .ForAll( x => 
                             numbers
                                 .AsParallel()
                                 .ForAll( i => MailboxManager.Send( x, i ) )
                    );
            };
        
        private It should_have_created_first_list_in_order = () => Values["one"].ToList().All( x => numbers.Contains( x ) );
    }
}