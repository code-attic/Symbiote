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

namespace Symbiote.Messaging.Impl.Channels

open System
open Symbiote.Messaging
open Symbiote.Messaging.Utility
open Symbiote.Messaging.Impl
open Symbiote.Messaging.Impl.Dispatch

type LocalChannel (dispatcher: IDispatcher) =
    
    member x.Dispatcher
        with get() = dispatcher

    interface IChannel with
        member x.Send<'TMessage when 'TMessage : not struct> (message: 'TMessage) =
            let correlate = castAs<ICorrelate>(message)
            let correlationId = match correlate with
                                | :? ICorrelate as c -> c.CorrelationId
                                | _ -> null
            let envelope = new DefaultEnvelope<'TMessage>(Guid.NewGuid(), correlationId, "NA", message) :> IEnvelope<'TMessage>
            dispatcher.Send envelope
