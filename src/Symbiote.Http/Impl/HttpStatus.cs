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
using System.Collections.Generic;
using Symbiote.Core.Extensions;

namespace Symbiote.Http.Impl
{
    public class HttpStatus
    {
        private static readonly string TEMPLATE = "{0} {1}";

        public static readonly HttpStatus Continue = new HttpStatus( 100, "Continue" );
        public static readonly HttpStatus SwitchingProtocols = new HttpStatus( 101, "Switching Protocols" );
        public static readonly HttpStatus Ok = new HttpStatus( 200, "OK" );
        public static readonly HttpStatus Created = new HttpStatus( 201, "Created" );
        public static readonly HttpStatus Accepted = new HttpStatus( 202, "Accepted" );
        public static readonly HttpStatus NoContent = new HttpStatus( 204, "No Content" );
        public static readonly HttpStatus MultipleChoices = new HttpStatus( 300, "Multiple Choices" );
        public static readonly HttpStatus MovedPermanently = new HttpStatus( 301, "Moved Permanently" );
        public static readonly HttpStatus UseProxy = new HttpStatus( 305, "Use Proxy" );
        public static readonly HttpStatus BadRequest = new HttpStatus( 400, "Bad Request" );
        public static readonly HttpStatus Unauthorized = new HttpStatus( 401, "Unauthorized" );
        public static readonly HttpStatus Forbidden = new HttpStatus( 403, "Forbidden" );
        public static readonly HttpStatus NotFound = new HttpStatus( 404, "Not Found" );
        public static readonly HttpStatus MethodNotAllowed = new HttpStatus( 405, "Method Not Allowed" );
        public static readonly HttpStatus RequestTimeout = new HttpStatus( 408, "Request Timeout" );
        public static readonly HttpStatus Conflict = new HttpStatus( 409, "Conflict" );
        public static readonly HttpStatus InternalServerError = new HttpStatus( 500, "Internal Server Error" );
        public static readonly HttpStatus NotImplemented = new HttpStatus( 501, "Not Implemented" );
        public static readonly HttpStatus ServiceNotAvailable = new HttpStatus( 503, "Service Not Available" );

        public static readonly Dictionary<string, HttpStatus> Lookup = 
            new Dictionary<string, HttpStatus>
            {
                { Continue.ToString(), Continue },
                { SwitchingProtocols.ToString(), SwitchingProtocols },
                { Ok.ToString(), Ok },
                { Created.ToString(), Created },
                { Accepted.ToString(), Accepted },
                { NoContent.ToString(), NoContent },
                { MultipleChoices.ToString(), MultipleChoices },
                { MovedPermanently.ToString(), MovedPermanently },
                { UseProxy.ToString(), UseProxy },
                { BadRequest.ToString(), BadRequest },
                { Unauthorized.ToString(), Unauthorized },
                { Forbidden.ToString(), Forbidden },
                { NotFound.ToString(), NotFound },
                { MethodNotAllowed.ToString(), MethodNotAllowed },
                { RequestTimeout.ToString(), RequestTimeout },
                { Conflict.ToString(), Conflict },
                { InternalServerError.ToString(), InternalServerError },
                { ServiceNotAvailable.ToString(), ServiceNotAvailable },
            };

        public static readonly Dictionary<int, HttpStatus> CodeLookup =
            new Dictionary<int, HttpStatus>
            {
                { Continue.Code, Continue },
                { SwitchingProtocols.Code, SwitchingProtocols },
                { Ok.Code, Ok },
                { Created.Code, Created },
                { Accepted.Code, Accepted },
                { NoContent.Code, NoContent },
                { MultipleChoices.Code, MultipleChoices },
                { MovedPermanently.Code, MovedPermanently },
                { UseProxy.Code, UseProxy },
                { BadRequest.Code, BadRequest },
                { Unauthorized.Code, Unauthorized },
                { Forbidden.Code, Forbidden },
                { NotFound.Code, NotFound },
                { MethodNotAllowed.Code, MethodNotAllowed },
                { RequestTimeout.Code, RequestTimeout },
                { Conflict.Code, Conflict },
                { InternalServerError.Code, InternalServerError },
                { ServiceNotAvailable.Code, ServiceNotAvailable },
            };

        public int Code { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return TEMPLATE.AsFormat( Code, Description );
        }

        public static implicit operator HttpStatus(System.Net.HttpStatusCode code)
        {
            return CodeLookup[(int)code];
        }

        public HttpStatus( int code, string description )
        {
            Code = code;
            Description = description;
        }
    }
}