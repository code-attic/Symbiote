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

namespace Symbiote.Http.Impl
{
    public interface IDefineHeaders
    {
        IDefineHeaders Accept( params string[] contentTypes );
        IDefineHeaders AcceptCharset( params string[] charSets );
        IDefineHeaders AcceptEncoding( params string[] encodings );
        IDefineHeaders AcceptLanguage( params string[] languages );
        IDefineHeaders Age( int seconds );
        IDefineHeaders Allow( params string[] methods );
        IDefineHeaders Authorization( string credentials );
        IDefineHeaders ContentEncoding( string encoding );
        IDefineHeaders ContentLanguage( string language );
        IDefineHeaders ContentLength( long length );
        IDefineHeaders ContentMD5( byte[] md5 );
        IDefineHeaders ContentType( string mediatype );
        IDefineHeaders Date( DateTime generated );
        IDefineHeaders RedirectTo( string locationUri );
        IDefineHeaders MaxForwards( int forwardLimit );
        IDefineHeaders ProxyAuthenticate( string challenge );
        IDefineHeaders ProxyAuthorization( string credentials );
        IDefineHeaders Server( string product );
        IDefineHeaders TransferEncoding( string transferEncoding );
        IDefineHeaders Upgrade( params string[] protocols );
        IDefineHeaders WwwAuthenticate( string challenge );
    }
}