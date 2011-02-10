// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using System.IO;
using Symbiote.Core.Futures;

namespace Symbiote.Core.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ReadToEnd(this Stream stream, int timeOut)
        {
            int read;
            var buffer = new byte[8*1024];
            if ( stream.CanTimeout )
                stream.ReadTimeout = timeOut;
            using( var memoryStream = new MemoryStream() )
            {
                while ( ((read = stream.Read( buffer, 0, buffer.Length )) > 0) )
                {
                    memoryStream.Write( buffer, 0, read );
                }
                return memoryStream.ToArray();
            }
        }

        public static void ReadAsync(this Stream stream, int timeout, Action<byte[]> onComplete )
        {
            var memoryStream = new MemoryStream();
            var buffer = new byte[8 * 1024];
            var read = 0;
            if (stream.CanTimeout)
                stream.ReadTimeout = timeout;

            Future.Of(
                x => stream.BeginRead(buffer, 0, buffer.Length, x, null),
                x =>
                    {
                        read = stream.EndRead(x);
                        if (read > 0)
                        {
                            memoryStream.Write(buffer, 0, read);
                        }
                        else
                        {
                            onComplete(memoryStream.ToArray());
                            memoryStream.Close();
                            memoryStream.Dispose();
                        }
                        return read;
                })
                .LoopWhile( () => read > 0)
                .Start();
        }
    }
}