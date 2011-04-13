using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
namespace Symbiote.Net
{
	public class LoopScheduler
		: TaskScheduler
	{
		public ConcurrentQueue<Task> Tasks { get; set; }
		
		protected override void QueueTask (Task task)
		{
			Tasks.Enqueue( task );
		}
		
		protected override IEnumerable<Task> GetScheduledTasks ()
		{
			return Tasks.ToArray();
		}
		
		protected override bool TryDequeue (Task task)
		{
			return Tasks.TryDequeue( out task );
		}
		
		protected override bool TryExecuteTaskInline (Task task, bool taskWasPreviouslyQueued)
		{
			return false;
		}
		
		public LoopScheduler ()
		{
			
		}
	}
}

