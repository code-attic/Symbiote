using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Messaging.Impl.Mesh
{
    public interface IInitializeNode
    {
        void InitializeChannels();
    }

    public class NullInitializer
        : IInitializeNode
    {
        public void InitializeChannels()
        {
            // do nothing
        }
    }
}
