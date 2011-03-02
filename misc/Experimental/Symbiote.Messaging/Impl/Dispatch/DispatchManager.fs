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
open System.Collections.Concurrent
open Symbiote.Core
open Symbiote.Core.Reflection
open Symbiote.Messaging
open Symbiote.Messaging.Impl.Channels
open Symbiote.Messaging.Impl.Actors
open Microsoft.Practices.ServiceLocation


type DispatchManager() =
    let dispatchers = new ConcurrentDictionary<Type, IDispatchMessage>()
    let init() =
        if dispatchers.Count = 0 then
            ServiceLocator.Current.GetAllInstances<IDispatchMessage>()
            |> Seq.iter (fun (x: IDispatchMessage) -> 
                for y in x.Handles do 
                    dispatchers.AddOrUpdate(y, x, (fun t m -> x)) 
                    |> ignore 
                )
    do
        init()

    let sendToHandler (id: string) (envelope: IEnvelope) =
        match dispatchers.TryGetValue envelope.MessageType with
        | true, dispatcher -> dispatcher.Dispatch(envelope)
        | _ -> printfn "No dispatcher for message :("
    
    let director = new Director<IEnvelope>(sendToHandler)

    member this.Director
        with get() = director

    interface IDispatcher with
        member x.Send<'TMessage when 'TMessage : not struct> (envelope: IEnvelope<'TMessage>) =
            let id = if String.IsNullOrEmpty envelope.CorrelationId then (envelope.MessageId.ToString()) else envelope.CorrelationId
            director.SendTo id envelope




