using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Machine.Specifications;
using Symbiote.Core.Fibers;

namespace Core.Tests.Fibers
{
    public abstract class with_director
    {
        public static Director<int> director;

        public static Dictionary<string, List<int>> Values;

        private Establish context = () =>
            {
                Values = new Dictionary<string, List<int>>()
                    {
                        {"", new List<int>()},
                        {"one", new List<int>()},
                        {"two", new List<int>()},
                        {"three", new List<int>()},
                    };

                director = new Director<int>( ( id, m ) => Values[id].Add( m ) );
            };
    }

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
                            .ForAll( i => director.Send( x, i ) )
                    );
            };

        private It should_have_created_first_list_in_order = () => Values["one"].ShouldEqual( numbers );
    }

    public class when_writing_high_volume_to_single_mailbox
        : with_director
    {
        private static List<int> numbers;
        private static Stopwatch watch;
        private static int total = 1000000;

        private Because of = () =>
            {
                numbers = Enumerable.Range( 1, total ).ToList();

                watch = Stopwatch.StartNew();

                numbers.ForEach( x => director.Send( x ) );

                watch.Stop();
            };

        private It should_finish_in_1_second = () => 
            watch.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 1000 );
        private It should_complete_in_order = () => 
            Values[""].ShouldEqual( numbers );
        private It should_have_correct_count = () => Values[""].Count().ShouldEqual( total );
    }

    public abstract class with_director_throwing_exceptions
    {
        public static Director<int> director;

        public static Dictionary<string, List<int>> Values;

        private Establish context = () =>
            {
                Values = new Dictionary<string, List<int>>()
                    {
                        {"", new List<int>()},
                        {"one", new List<int>()},
                        {"two", new List<int>()},
                        {"three", new List<int>()},
                    };

                director = new Director<int>( ( id, m ) =>
                    {
                        if( m % 3 == 0 )
                            throw new Exception( "AHHH!" );
                        else
                            Values[id].Add( m );
                    } );
            };
    }

    public class when_throwing_exception_during_mailbox_process
        : with_director_throwing_exceptions
    {
        static List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        static List<int> results = new List<int>() { 1, 2, 4, 5, 7, 8, 10 };

        private Because of = () =>
            {
                numbers.ForEach( x => director.Send( x ) );
            };

        private It should_have_correct_result = () => Values[""].ShouldEqual( results );
        private It should_have_seven_results = () => Values[""].Count.ShouldEqual( 7 );
    }
}
