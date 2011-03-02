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

namespace Symbiote.Messaging.Impl.Dispatch

open System
open System.Collections.Generic
open Symbiote.Messaging

type IDispatchMessage =
    interface
        abstract member ActorType : Type
            with get
        abstract member Handles : IEnumerable<Type>
            with get
        abstract member CanHandle : Object -> bool
        abstract member Dispatch : IEnvelope -> unit
    end


type IDispatchMessage<'TMessage when 'TMessage : not struct> =
    inherit IDispatchMessage


type IDispatchMessage<'TActor, 'TMessage when
    'TActor : not struct and 
    'TMessage :> ICorrelate and
    'TMessage : not struct> =
    interface end