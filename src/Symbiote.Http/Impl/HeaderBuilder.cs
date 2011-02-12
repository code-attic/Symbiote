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
using System.Linq;
using System.Text;

namespace Symbiote.Http.Impl
{
    public class HeaderBuilder
        : IDefineHeaders
    {
        public IDictionary<string, string> ResponseHeaders { get; set; }

        #region IDefineHeaders Members

        public IDefineHeaders Accept( params string[] contentTypes )
        {
            ResponseHeaders.Add( "Accept", string.Join( ", ", contentTypes ) );
            return this;
        }

        public IDefineHeaders AcceptCharset( params string[] charSets )
        {
            ResponseHeaders.Add( "Accept-Charset", string.Join( ", ", charSets ) );
            return this;
        }

        public IDefineHeaders AcceptEncoding( params string[] encodings )
        {
            ResponseHeaders.Add( "Accept-Encoding", string.Join( ", ", encodings ) );
            return this;
        }

        public IDefineHeaders AcceptLanguage( params string[] languages )
        {
            ResponseHeaders.Add( "Accept-Language", string.Join( ", ", languages ) );
            return this;
        }

        public IDefineHeaders Age( int seconds )
        {
            ResponseHeaders.Add( "Age", seconds.ToString() );
            return this;
        }

        public IDefineHeaders Allow( params string[] methods )
        {
            ResponseHeaders.Add( "Allow", string.Join( ", ", methods ) );
            return this;
        }

        public IDefineHeaders Authorization( string credentials )
        {
            ResponseHeaders.Add( "Authorization", credentials );
            return this;
        }

        public IDefineHeaders ContentEncoding( string encoding )
        {
            ResponseHeaders.Add( "Content-Encoding", encoding );
            return this;
        }

        public IDefineHeaders ContentLanguage( string language )
        {
            ResponseHeaders.Add( "Content-Language", language );
            return this;
        }

        public IDefineHeaders ContentLength( long length )
        {
            ResponseHeaders.Add( "ContentLength", length.ToString() );
            return this;
        }

        public IDefineHeaders ContentMD5( byte[] md5 )
        {
            ResponseHeaders.Add( "Content-MD5", Encoding.UTF8.GetString( md5 ) );
            return this;
        }

        public IDefineHeaders ContentType( string mediatype )
        {
            ResponseHeaders.Add( "Content-Type", mediatype );
            return this;
        }

        public IDefineHeaders Date( DateTime generated )
        {
            ResponseHeaders.Add( "Date", generated.ToString() );
            return this;
        }

        public IDefineHeaders RedirectTo( string locationUri )
        {
            ResponseHeaders.Add( "Redirect-To", locationUri );
            return this;
        }

        public IDefineHeaders MaxForwards( int forwardLimit )
        {
            ResponseHeaders.Add( "Max-Forwards", forwardLimit.ToString() );
            return this;
        }

        public IDefineHeaders ProxyAuthenticate( string challenge )
        {
            ResponseHeaders.Add( "Proxy-Authenticate", challenge );
            return this;
        }

        public IDefineHeaders ProxyAuthorization( string credentials )
        {
            ResponseHeaders.Add( "Proxy-Authorization", credentials );
            return this;
        }

        public IDefineHeaders Server( string product )
        {
            ResponseHeaders.Add( "Server", product );
            return this;
        }

        public IDefineHeaders TransferEncoding( string transferEncoding )
        {
            ResponseHeaders.Add( "Transfer-Encoding", transferEncoding );
            return this;
        }

        public IDefineHeaders Upgrade( params string[] protocols )
        {
            ResponseHeaders.Add( "Upgrade", string.Join(", ", protocols ) );
            return this;
        }

        public IDefineHeaders WwwAuthenticate( string challenge )
        {
            ResponseHeaders.Add( "WWW-Authentication", challenge );
            return this;
        }

        #endregion

        public HeaderBuilder( IDictionary<string, string> responseHeaders )
        {
            ResponseHeaders = responseHeaders;
        }
    }
}