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
using Symbiote.Http.Impl;

namespace Symbiote.Http
{
    public interface IBuildResponse
    {
        IBuildResponse AppendToBody( byte[] bytes );
        IBuildResponse AppendToBody( string text );
        IBuildResponse AppendFileContentToBody( string path );
        IBuildResponse AppendJson<T>( T item );
        IBuildResponse AppendProtocolBuffer<T>( T item );
        IBuildResponse DefineHeaders( Action<IDefineHeaders> headerDefinition );
        void Submit( string status );
        void Submit( HttpStatus status );
    }
}