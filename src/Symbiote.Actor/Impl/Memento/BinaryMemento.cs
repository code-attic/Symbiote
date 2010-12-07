using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Reflection;
using Symbiote.Core.Utility;

namespace Symbiote.Actor.Impl.Memento
{
    /// <summary>
    /// Behold and weep. Exists to motivate developers
    /// to write their own concrete mementos because if they don't
    /// this provides an awful default that will suck the performance
    /// right out of their app.
    /// </summary>
    public class BinaryMemento<T>
        : IMemento<T>
    {
        public BinaryFormatter BinarySerializer { get; set; }
        public MemoryStream Stream { get; set; }

        public void Capture( T instance )
        {
            Stream = new MemoryStream();
            BinarySerializer.Serialize( Stream, instance );
        }

        public void Reset( T instance )
        {
            instance = Retrieve();
        }

        public T Retrieve()
        {
            Stream.Position = 0;
            return (T) BinarySerializer.Deserialize( Stream );
        }

        public BinaryMemento()
        {
            BinarySerializer = new BinaryFormatter();
        }
    }
}