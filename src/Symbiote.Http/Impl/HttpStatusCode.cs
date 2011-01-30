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
namespace Symbiote.Http.Impl
{
    public class HttpStatusCode
    {
        public readonly string ACCEPTED = "202 Accepted";
        public readonly string BAD_REQUEST = "400 Bad Request";
        public readonly string CONFLICT = "409 Conflict";
        public readonly string CONTINUE = "100 Continue";
        public readonly string CREATED = "201 Created";
        public readonly string FORBIDDEN = "403 Forbidden";
        public readonly string METHOD_NOT_ALLOWED = "405 Method Not Allowed";
        public readonly string MOVED_PERMANENTLY = "301 Moved Permanently";
        public readonly string MULTIPLE_CHOICES = "300 Multiple Choices";
        public readonly string NOTFOUND = "404 Not Found";
        public readonly string NOT_IMPLEMENTED = "501 Not Implemented";
        public readonly string NO_CONTENT = "204 No Content";
        public readonly string OK = "200 OK";
        public readonly string SERVER_ERROR = "500 Internal Server Error";
        public readonly string SERVICE_UNAVAILABLE = "503 Service Not Available";
        public readonly string SWITCH_PROTOCOL = "101 Switching Protocols";
        public readonly string TIMEOUT = "408 Request Timeout";
        public readonly string UNAUTHORIZED = "401 Unauthorized";
        public readonly string USE_PROXY = "305 Use Proxy";
    }
}