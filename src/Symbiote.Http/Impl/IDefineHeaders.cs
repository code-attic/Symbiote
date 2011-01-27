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