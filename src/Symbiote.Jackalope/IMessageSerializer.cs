using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Jackalope
{
    public interface IMessageSerializer
    {
        T Deserialize<T>(byte[] message)
            where T : class;

        object Deserialize(byte[] message);

        byte[] Serialize<T>(T body)
            where T : class;
    }
}
