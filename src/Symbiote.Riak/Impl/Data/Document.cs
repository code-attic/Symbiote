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
namespace Symbiote.Riak.Impl.Data
{
    public class Document<T>
    {
        private readonly string DEFAULT_MIME_TYPE = "application/octet-stream";
        public string Charset { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public uint LastModified { get; set; }
        public uint LastModifiedInSeconds { get; set; }
        public T Value { get; set; }
        public string VectorClock { get; set; }

        public Document()
        {
        }

        public Document( T instance, string vectorClock )
        {
            Value = instance;
            Charset = null;
            ContentType = DEFAULT_MIME_TYPE;
            VectorClock = vectorClock;
        }
    }
}