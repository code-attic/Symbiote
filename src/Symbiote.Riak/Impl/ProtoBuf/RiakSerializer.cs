using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Symbiote.Core.Impl.Serialization;
using Symbiote.Riak.Impl.ProtoBuf.Data.Request;
using Symbiote.Riak.Impl.ProtoBuf.Data.Response;

namespace Symbiote.Riak.Impl.ProtoBuf
{
    public class RiakSerializer
    {
        public Dictionary<Type, short> CommandCodes = new Dictionary<Type, short>()
        {
            { typeof(Ping), 1 },
            { typeof(GetClientId), 3 },
            { typeof(SetClientId), 5 },
            { typeof(GetServerInfo), 7 },
            { typeof(Get), 9 },
            { typeof(Persist), 11 },
            { typeof(Delete), 13 },
            { typeof(ListBuckets), 15 },
            { typeof(ListKeys), 17 },
            { typeof(GetBucketProperties), 19 },
            { typeof(SetBucketProperties), 21 },
            { typeof(RunMapReduce), 23 },
        };

        public Dictionary<short, Type> ResponseCodes = new Dictionary<short, Type>()
        {
            {0, typeof(Error)},
            {2, typeof(Ping)},
            {4, typeof(Client)},
            {6, typeof(ClientIdSet)},
            {8, typeof(ServerInformation)},
            {10, typeof(GetResult)},
            {12, typeof(Persisted)},
            {14, typeof(Deleted)},
            {16, typeof(BucketList)},
            {18, typeof(KeyList)},
            {20, typeof(BucketProperties)},
            {22, typeof(BucketPropertiesSet)},
            {24, typeof(MapReduceResult)},
        };

        public byte[] GetCommandBytes<T>(T command)
        {
            var commandType = typeof(T);
            var code = CommandCodes[commandType];
            var byteCode = BitConverter.GetBytes( code )[1];
            var contentBytes = command.ToProtocolBuffer();
            var length = contentBytes.Length + 1;
            var byteLength = BitConverter.GetBytes( length );
            return byteLength
                .Concat( new [] {byteCode} )
                .Concat( contentBytes )
                .ToArray();
        }

        public object GetResult(Stream stream)
        {
            var bytes = stream.ToBytes();
            var messageType = ResponseCodes[bytes[4]];
            var messageBytes = bytes.Skip( 5 ).ToArray();
            if(messageBytes.Length == 0)
            {
                return Activator.CreateInstance( messageType );
            }
            else
            {
                return messageBytes.FromProtocolBuffer(messageType);   
            }
        }
    }
}
