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

open Symbiote.Core.Reflection
open Symbiote.Messaging
open Symbiote.Messaging.Impl.Actors
open Microsoft.Practices.ServiceLocation

type MessageDispatcher<'TMessage when 'TMessage : not struct>() =
    let handlesMessagesOf = 
        let chain = Reflector.GetInheritenceChain(typedefof<'TMessage>)
        match chain with
            | null -> 
                seq {
                    yield typedefof<'TMessage>
                }
            | _ ->
                seq {
                    yield typedefof<'TMessage>
                    for t in chain -> t
                }

    interface IDispatchMessage with
        
        member x.ActorType
            with get() = null
        member x.Handles
            with get() = handlesMessagesOf
        member x.CanHandle messageOf =
            match messageOf with
            | :? 'TMessage -> true
            | _ -> false
        member x.Dispatch envelope =
            let handler = ServiceLocator.Current.GetInstance<IHandle<'TMessage>>()
            handler.Handle (envelope :?> IEnvelope<'TMessage>)

    interface IDispatchMessage<'TMessage>
    

type MessageDispatcher<'TActor, 'TMessage when
    'TActor : not struct and 
    'TMessage :> ICorrelate and
    'TMessage : not struct>(agency: IAgency) =
    let agent = agency.GetAgentFor<'TActor>()
    let handlesMessagesOf = 
        let chain = Reflector.GetInheritenceChain(typedefof<'TMessage>)
        match chain with
            | null -> 
                seq {
                    yield typedefof<'TMessage>
                }
            | _ ->
                seq {
                    yield typedefof<'TMessage>
                    for t in chain -> t
                }

    interface IDispatchMessage with
        
        member x.ActorType
            with get() = null
        member x.Handles
            with get() = handlesMessagesOf
        member x.CanHandle messageOf =
            match messageOf with
            | :? 'TMessage -> true
            | _ -> false
        member x.Dispatch envelope =
            let actor = agent.GetActor envelope.CorrelationId
            let handler = ServiceLocator.Current.GetInstance<IHandle<'TActor, 'TMessage>>()
            handler.Handle actor (envelope :?> IEnvelope<'TMessage>)
            agent.Memoize actor

    interface IDispatchMessage<'TActor, 'TMessage>