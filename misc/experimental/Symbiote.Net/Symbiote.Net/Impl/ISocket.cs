
using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Symbiote.Core.Locking;
using Symbiote.Core;
namespace Symbiote.Net
{
	public interface ISocket
	{
		void EnqueueRead( Action<ArraySegment<byte>> onBytes, Action<Exception> onException );
		void EnqueueWrite( ArraySegment<byte> segment, Action onComplete, Action<Exception> onException );
	}
}
