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

using Symbiote.Core.Extensions;

namespace Symbiote.Net
{
    public abstract class AuthRequiredResponse
    {
        public const string HTTP_HEADER_TAG = @"HTTP/{0} 401 Authorization Required";
        public const string DEFAULT_HTTP_VERSION = "1.1";
        public const string SERVER_TAG = @"Server: HTTPd/{0}";
        public const string WWW_AUTH_TAG = @"WWW-Authenticate: {0}";
        public const string CONTENT_TYPE = @"Content-Type: text/html";
        public const string CONTENT_LENGTH = @"Content-Length: 311";

        public const string HTML_AUTH_REQ_RESPONSE = @"
<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN""
 ""http://www.w3.org/TR/1999/REC-html401-19991224/loose.dtd"">
<HTML>
  <HEAD>
    <TITLE>Error</TITLE>
    <META HTTP-EQUIV=""Content-Type"" CONTENT=""text/html; charset=ISO-8859-1"">
  </HEAD>
  <BODY><H1>401 Unauthorized.</H1></BODY>
</HTML>";

        public string HttpVersion { get; set; }

        public abstract string BuildAuthenticateString();

        public override string ToString()
        {
            return string.Join(
                @"\r\n",
                HTTP_HEADER_TAG.AsFormat(HttpVersion ?? DEFAULT_HTTP_VERSION),
                SERVER_TAG.AsFormat(HttpVersion ?? DEFAULT_HTTP_VERSION), 
                BuildAuthenticateString(),
                CONTENT_TYPE,
                CONTENT_LENGTH,
                HTML_AUTH_REQ_RESPONSE
                );
        }
    }
}