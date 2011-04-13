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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Serialization;
using Symbiote.Core.Utility;
using Symbiote.Http.Config;
using Symbiote.Http.NetAdapter.Socket;
using Symbiote.Http.ViewAdapter;
using System.Threading;

namespace Symbiote.Http.Owin.Impl
{
    public class ResponseHelper :
        IBuildResponse,
        IOwinObservable
    {
        public readonly string RENDER_EXCEPTION_TEMPLATE = @"
<html>
    <head>
        <title>Symbiote.Http View Rendering Engine Exception</title>
        <h1>Wow, this is so embarassing...</h1>
        <p>
        <pre>
        {0}
        </pre>
        </p>
    </head>
";
        public HttpWebConfiguration Configuration { get; set; }
        
        public Func<ArraySegment<byte>, Action, bool> OnNextWrite { get; set; }
        public Action<Exception> OnWriteError { get; set; }
        public Action OnWriteComplete { get; set; }

        public IDictionary<string, string> ResponseHeaders { get; set; }
        public List<object> ResponseChunks { get; set; }
        public Func<string, byte[]> Encoder { get; set; }
        public IDefineHeaders HeaderDefinitions { get; set; }
		public bool Disposed { get; protected set; }
		public ManualResetEventSlim PendingWrite { get; protected set; }

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
            path = path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine( Configuration.BaseContentPath, path );
            if ( !File.Exists( fullPath ) )
            {
                throw new FileNotFoundException( "Requested file could not be found: '{0}'".AsFormat( fullPath ) );
            }

            using( var stream = new FileStream( fullPath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
            {
                var bytes = stream.Length;
                var buffer = new byte[bytes];
                stream.Read( buffer, 0, (int) bytes );
                AppendToBody( buffer );
                var contentType = GetContentTypeFromPath( fullPath );
                HeaderDefinitions.ContentType( contentType );
            }
            return this;
        }

        public IBuildResponse AppendJson<T>( T item )
        {
            return AppendToBody( item.ToJson() );
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

        public IBuildResponse RenderView<TModel>( TModel model, string viewName )
        {
            if( !string.IsNullOrEmpty( Configuration.DefaultLayoutTemplate ) )
                return RenderView( model, viewName, Configuration.DefaultLayoutTemplate );

            var engine = Assimilate.GetInstanceOf<IViewEngine>();
            using( var stream = new MemoryStream() )
            using( var streamWriter = new StreamWriter( stream, Encoding.UTF8 ) )
            {
                try
                {
                    engine.Render( viewName, model, streamWriter );
                }
                catch ( Exception e )
                {
                    streamWriter.Write( RENDER_EXCEPTION_TEMPLATE.AsFormat( e ) );
                    streamWriter.Flush();
                }
                streamWriter.Flush();
                DefineHeaders( x => x.ContentType( ContentType.Html ).ContentLength( stream.Length ) );
                AppendToBody( stream.GetBuffer() );
            }
            return this;
        }

        public IBuildResponse RenderView<TModel>( TModel model, string viewName, string layoutName )
        {
            var engine = Assimilate.GetInstanceOf<IViewEngine>();
            using( var stream = new MemoryStream() )
            using( var streamWriter = new StreamWriter( stream, Encoding.UTF8 ) )
            {
                try
                {
                    engine.Render( viewName, layoutName, model, streamWriter );
                }
                catch ( Exception e )
                {
                    streamWriter.Write( RENDER_EXCEPTION_TEMPLATE.AsFormat( e ) );
                    streamWriter.Flush();
                }
                streamWriter.Flush();
                DefineHeaders( x => x.ContentType( ContentType.Html ).ContentLength( stream.Length ) );
                AppendToBody( stream.GetBuffer() );
            }
            return this;
        }

        public void RenderException( Exception ex, Stream stream )
        {

        }

        public Action Setup( IOwinObserver observer )
        {
            return Setup( observer.OnNext, observer.OnError, observer.OnComplete );
        }

        public Action Setup( Func<ArraySegment<byte>, Action, bool> onNext, Action<Exception> onError, Action complete )
        {
            OnNextWrite = onNext;
            OnWriteError = onError;
            OnWriteComplete = complete;
            return Stop;
        }

        public void Stop()
        {
			Dispose();
        }

        public void Submit( string status )
        {
            var httpStatus = HttpStatus.Lookup[status];

            var builder = new DelimitedBuilder("\r\n");
            var headerBuilder = new HeaderBuilder( ResponseHeaders );
            var responseBody = new MemoryStream();

            ResponseChunks.ForEach( x => ResponseEncoder.Write( x, responseBody ) );

            builder.AppendFormat( "HTTP/1.1 {0}", status );
            headerBuilder.ContentLength( responseBody.Length );
            headerBuilder.Date( DateTime.UtcNow );

            ResponseHeaders.ForEach( x => builder.AppendFormat( "{0}: {1}", x.Key, x.Value ) );
            builder.Append( "\r\n" );
            var header = builder.ToString();
            var headerBuffer = Encoding.UTF8.GetBytes( header );
            var bodyBuffer = responseBody.GetBuffer();
			
			PendingWrite.Reset();
            OnNextWrite( new ArraySegment<byte>(headerBuffer), () => WriteBody( bodyBuffer ) );
        }

        private void WriteBody( byte[] bodyBuffer )
        {
            OnNextWrite( new ArraySegment<byte>( bodyBuffer ), () => { PendingWrite.Set(); } );
            OnWriteComplete();
        }

        public void Submit( HttpStatus status )
        {
            Submit( status.ToString() );
        }

        private string GetContentTypeFromPath( string path )
        {
            string extension = Path.GetExtension( path ).Replace( ".", "" );
            string contentType;
            if ( !ContentType.ContentByExtension.TryGetValue( extension, out contentType ) )
                contentType = ContentType.Binary;
            return contentType;
        }

        public IBuildResponse EncodeStringsWith( Func<string, byte[]> encoder )
        {
            Encoder = encoder;
            return this;
        }

        public ResponseHelper( HttpWebConfiguration configuration )
        {
            Configuration = configuration;
            ResponseHeaders = new Dictionary<string, string>();
            ResponseChunks = new List<object>();
            Encoder = x => Encoding.UTF8.GetBytes( x );
            HeaderDefinitions = new HeaderBuilder( ResponseHeaders );
			PendingWrite = new ManualResetEventSlim( true );
        }
		
		public void Dispose ()
		{
			if( !Disposed )
			{
				PendingWrite.Wait();
				Disposed = true;
				OnNextWrite = null;
				OnWriteError = null;
				OnWriteComplete = null;
				Encoder = null;
				PendingWrite.Dispose();
			}
		}
    }
}