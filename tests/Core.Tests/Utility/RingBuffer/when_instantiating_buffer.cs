using Machine.Specifications;

namespace Core.Tests.Utility.RingBuffer
{
    public class when_instantiating_buffer
        : with_buffer
    {
        public static int count = 1;
        private Because of = () =>
        {
            Buffer.AddTransform( x => x );
            Buffer.AddTransform( x => x );
            Buffer.AddTransform( x => x );
            var cell = Buffer.Head;
            do
            {
                cell = cell.Next;
                count++;
            } while ( !cell.Tail );
        };

        private It should_have_correct_cell_count = () => count.ShouldEqual(Size);
        private It should_have_correct_previous_step_for_first_element = () => Buffer.PreviousStepLookup[0].ShouldEqual(3);
        private It should_have_correct_previous_step_for_last_element = () => Buffer.PreviousStepLookup[3].ShouldEqual(2);
    }
}