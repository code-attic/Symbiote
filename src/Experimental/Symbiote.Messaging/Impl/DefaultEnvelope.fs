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

namespace Symbiote.Messaging.Impl

open System
open Symbiote.Messaging

type DefaultEnvelope<'TMessage when 'TMessage : not struct> 
    (id: Guid,
     correlationId: string,
     returnTo: string,
     message: 'TMessage) =

    interface IEnvelope<'TMessage> with
        member x.CorrelationId
            with get() = correlationId
        member x.MessageId
            with get() = id
        member x.MessageType
            with get() = typedefof<'TMessage>
        member x.Message
            with get() = message
        member x.ReturnTo
            with get() = returnTo
    end