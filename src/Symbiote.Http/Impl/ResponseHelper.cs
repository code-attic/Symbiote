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
using System.IO;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Impl.Serialization;

namespace Symbiote.Http.Impl
{
    public class ResponseHelper
        : IBuildResponse
    {
        public Action<string, IDictionary<string, IList<string>>, IEnumerable<object>> Respond { get; set; }
        public IDictionary<string, IList<string>> ResponseHeaders { get; set; }
        public List<object> ResponseChunks { get; set; }
        public Func<string, byte[]> Encoder { get; set; }
        public IDefineHeaders HeaderDefinitions { get; set; }
        public string FileTemplate = @"..\..{0}";

        public IBuildResponse AppendToBody( byte[] bytes )
        {
            ResponseChunks.Add( bytes );
            return this;
        }

        public IBuildResponse AppendToBody( string text )
        {
            return AppendToBody( Encoder( text ) );
        }

        public IBuildResponse AppendFileContentToBody( string path )
        {
            var fullPath = FileTemplate.AsFormat( path.Replace( "/", @"\" ) );
            if(!File.Exists( fullPath ))
            {
                throw new FileNotFoundException( "Requested file could not be found: '{0}'".AsFormat( fullPath ) );
            }

            using(var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var bytes = stream.Length;
                var buffer = new byte[bytes];
                stream.Read( buffer, 0, (int) bytes );
                AppendToBody( buffer );
                var contentType = GetContentTypeFromPath(fullPath);
                HeaderDefinitions.ContentType( contentType );
            }
            return this;
        }

        private string GetContentTypeFromPath( string path )
        {
            string extension = Path.GetExtension( path ).Replace( ".","" );
            string contentType;
            if (!ContentType.ContentByExtension.TryGetValue(extension, out contentType))
                contentType = ContentType.Binary;
            return contentType;
        }

        public IBuildResponse AppendJson<T>( T item )
        {
            return AppendToBody(item.ToJson());
        }

        public IBuildResponse AppendProtocolBuffer<T>( T item )
        {
            return AppendToBody( item.ToProtocolBuffer() );
        }

        public IBuildResponse DefineHeaders( Action<IDefineHeaders> headerDefinition )
        {
            headerDefinition( HeaderDefinitions );
            return this;
        }

        public IBuildResponse EncodeStringsWith(Func<string, byte[]> encoder)
        {
            Encoder = encoder;
            return this;
        }

        public void Submit(string status)
        {
            Respond( status, ResponseHeaders, ResponseChunks );
        }

        public void Submit(HttpStatus status)
        {
            Submit( status.ToString() );
        }

        public static implicit operator ResponseHelper(Action<string, IDictionary<string, IList<string>>, IEnumerable<object>> respond)
        {
            return new ResponseHelper( respond );
        }

        public ResponseHelper( Action<string, IDictionary<string, IList<string>>, IEnumerable<object>> respond )
        {
            Respond = respond;
            ResponseHeaders = new Dictionary<string, IList<string>>();
            ResponseChunks = new List<object>();
            Encoder = x => Encoding.UTF8.GetBytes( x );
            HeaderDefinitions = new HeaderBuilder(ResponseHeaders);
        }
    }
}