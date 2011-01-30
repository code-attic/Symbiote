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
        public IDictionary<string, IList<string>> ResponseHeaders { get; set; }

        #region IDefineHeaders Members

        public IDefineHeaders Accept( params string[] contentTypes )
        {
            ResponseHeaders.Add( "Accept", contentTypes.ToList() );
            return this;
        }

        public IDefineHeaders AcceptCharset( params string[] charSets )
        {
            ResponseHeaders.Add( "Accept-Charset", charSets.ToList() );
            return this;
        }

        public IDefineHeaders AcceptEncoding( params string[] encodings )
        {
            ResponseHeaders.Add( "Accept-Encoding", encodings.ToList() );
            return this;
        }

        public IDefineHeaders AcceptLanguage( params string[] languages )
        {
            ResponseHeaders.Add( "Accept-Language", languages.ToList() );
            return this;
        }

        public IDefineHeaders Age( int seconds )
        {
            ResponseHeaders.Add( "Age", new List<string>
                                            {
                                                seconds.ToString()
                                            } );
            return this;
        }

        public IDefineHeaders Allow( params string[] methods )
        {
            ResponseHeaders.Add( "Allow", methods.ToList() );
            return this;
        }

        public IDefineHeaders Authorization( string credentials )
        {
            ResponseHeaders.Add( "Authorization", new List<string>
                                                      {
                                                          credentials
                                                      } );
            return this;
        }

        public IDefineHeaders ContentEncoding( string encoding )
        {
            ResponseHeaders.Add( "Content-Encoding", new List<string>
                                                         {
                                                             encoding
                                                         } );
            return this;
        }

        public IDefineHeaders ContentLanguage( string language )
        {
            ResponseHeaders.Add( "Content-Language", new List<string>
                                                         {
                                                             language
                                                         } );
            return this;
        }

        public IDefineHeaders ContentLength( long length )
        {
            ResponseHeaders.Add( "ContentLength", new List<string>
                                                      {
                                                          length.ToString()
                                                      } );
            return this;
        }

        public IDefineHeaders ContentMD5( byte[] md5 )
        {
            ResponseHeaders.Add( "Content-MD5", new List<string>
                                                    {
                                                        Encoding.UTF8.GetString( md5 )
                                                    } );
            return this;
        }

        public IDefineHeaders ContentType( string mediatype )
        {
            ResponseHeaders.Add( "Content-Type", new List<string>
                                                     {
                                                         mediatype
                                                     } );
            return this;
        }

        public IDefineHeaders Date( DateTime generated )
        {
            ResponseHeaders.Add( "Date", new List<string>
                                             {
                                                 generated.ToString()
                                             } );
            return this;
        }

        public IDefineHeaders RedirectTo( string locationUri )
        {
            ResponseHeaders.Add( "Redirect-To", new List<string>
                                                    {
                                                        locationUri
                                                    } );
            return this;
        }

        public IDefineHeaders MaxForwards( int forwardLimit )
        {
            ResponseHeaders.Add( "Max-Forwards", new List<string>
                                                     {
                                                         forwardLimit.ToString()
                                                     } );
            return this;
        }

        public IDefineHeaders ProxyAuthenticate( string challenge )
        {
            ResponseHeaders.Add( "Proxy-Authenticate", new List<string>
                                                           {
                                                               challenge
                                                           } );
            return this;
        }

        public IDefineHeaders ProxyAuthorization( string credentials )
        {
            ResponseHeaders.Add( "Proxy-Authorization", new List<string>
                                                            {
                                                                credentials
                                                            } );
            return this;
        }

        public IDefineHeaders Server( string product )
        {
            ResponseHeaders.Add( "Server", new List<string>
                                               {
                                                   product
                                               } );
            return this;
        }

        public IDefineHeaders TransferEncoding( string transferEncoding )
        {
            ResponseHeaders.Add( "Transfer-Encoding", new List<string>
                                                          {
                                                              transferEncoding
                                                          } );
            return this;
        }

        public IDefineHeaders Upgrade( params string[] protocols )
        {
            ResponseHeaders.Add( "Upgrade", protocols.ToList() );
            return this;
        }

        public IDefineHeaders WwwAuthenticate( string challenge )
        {
            ResponseHeaders.Add( "WWW-Authentication", new List<string>
                                                           {
                                                               challenge
                                                           } );
            return this;
        }

        #endregion

        public HeaderBuilder( IDictionary<string, IList<string>> responseHeaders )
        {
            ResponseHeaders = responseHeaders;
        }
    }
}