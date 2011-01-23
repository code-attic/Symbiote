/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Symbiote.Core.Impl.Serialization;
using Symbiote.Riak.Impl.ProtoBuf.Request;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl.ProtoBuf
{
    public class RiakSerializer
    {
        public Dictionary<Type, RequestCode> CommandCodes = new Dictionary<Type, RequestCode>()
        {
            { typeof(Ping), RequestCode.Ping },
            { typeof(GetClientId), RequestCode.GetClientId },
            { typeof(SetClientId), RequestCode.SetClientId },
            { typeof(GetServerInfo), RequestCode.GetServerInfo },
            { typeof(Get), RequestCode.Get },
            { typeof(Persist), RequestCode.Persist },
            { typeof(Delete), RequestCode.Delete },
            { typeof(ListBuckets), RequestCode.ListBuckets },
            { typeof(ListKeys), RequestCode.ListKeys },
            { typeof(GetBucketProperties), RequestCode.GetBucketProperties },
            { typeof(SetBucketProperties), RequestCode.SetBucketProperties },
            { typeof(RunMapReduce), RequestCode.RunMapReduce },
        };

        public Dictionary<short, Type> ResponseCodes = new Dictionary<short, Type>()
        {
            { 0, typeof(Error) },
            { 2, typeof(Ping) },
            { 4, typeof(Client) },
            { 6, typeof(ClientIdSet) },
            { 8, typeof(ServerInformation) },
            { 10, typeof(GetResult) },
            { 12, typeof(Persisted) },
            { 14, typeof(Deleted) },
            { 16, typeof(BucketList) },
            { 18, typeof(KeyList) },
            { 20, typeof(BucketProperties) },
            { 22, typeof(BucketPropertiesSet) },
            { 24, typeof(MapReduceResult) },
        };

        public byte[] GetCommandBytes<T>( T command )
        {
            var commandType = typeof(T);
            var code = CommandCodes[commandType];
            var byteCode = Convert.ToByte( code );
            var contentBytes = command.ToProtocolBuffer();
            var length = IPAddress.HostToNetworkOrder(contentBytes.Length + 1);
            byte[] bytes;
            if(contentBytes.Length > 0)
            {
                bytes = BitConverter
                    .GetBytes((length))
                    .Concat(new[] { byteCode })
                    .Concat(contentBytes)
                    .ToArray();
            }
            else
            {
                bytes = BitConverter
                    .GetBytes(IPAddress.HostToNetworkOrder(1))
                    .Concat(new[] { byteCode })
                    .ToArray();
            }
            return bytes;
        }

        public object GetResult( NetworkStream stream )
        {
            var bytes = stream.ToBytes();
            if(bytes.Length > 4)
            {
                var messageType = ResponseCodes[bytes[4]];
                var messageBytes = bytes.Skip( 5 ).ToArray();
                if ( messageBytes.Length == 0 )
                {
                    return Activator.CreateInstance( messageType );
                }
                else
                {
                    return messageBytes.FromProtocolBuffer( messageType );
                }
            }
            else
            {
                return null;
            }
        }
    }
}