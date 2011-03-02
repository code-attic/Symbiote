(* 
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
*)

namespace Symbiote.Messaging

open System

type IEnvelope =
    interface
        abstract member MessageId : Guid
            with get
        abstract member CorrelationId : string
            with get
        abstract member ReturnTo : string
            with get
        abstract member MessageType : Type
            with get
    end


type IEnvelope<'TMessage when 'TMessage : not struct> =
    inherit IEnvelope

    abstract member Message : 'TMessage 
            with get

