﻿// /* 
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
using Symbiote.Core;
using Symbiote.Http.Config;
using Symbiote.Http.NetAdapter.Socket;
using Symbiote.Http.Owin;
using Symbiote.Http;
using Symbiote.Http.Owin.Impl;

namespace Symbiote.Http
{
    public interface IApplication
        : IOwinObserver
    {
        Action Cancel { get; set; }
        IRequest Request { get; set; }
        IBuildResponse Response { get; set; }
        void Process( IRequest request, IBuildResponse response, Action<Exception> onException );
    }
}