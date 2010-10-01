using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Retlang.Channels;
using Retlang.Fibers;

namespace Jackalope.Tests.RetLanging
{
    public class ChannelBus<TMessage>
    {
        protected IChannel<TMessage> Channel { get; set; }
        protected ConcurrentQueue<IFiber> Fibers { get; set; }


    }

    public class Matrix<TKey, TMessage>
    {
        public ConcurrentDictionary<TKey, ChannelBus<TMessage>> Channels { get; set; }
        
    }
}
