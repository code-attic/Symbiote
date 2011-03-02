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

namespace Symbiote.Messaging.Impl.Actors

open System
open System.Collections.Concurrent
open Symbiote.Messaging.Impl

type InMemoryActorCache (keyAccessor: IKeyAccessor) =
    let actors = new ConcurrentDictionary<string, Object>()

    member x.Actors 
        with get() = actors
    member x.KeyAccessor
        with get() = keyAccessor

    interface IActorCache with
        
        member x.GetOrAdd<'TActor, 'TKey when 'TActor : not struct> (id: 'TKey) (createWith: ('TKey -> 'TActor)) =
            match x.Actors.TryGetValue (id.ToString()) with
            | true, actor -> actor :?> 'TActor
            | _ -> 
                let actor = createWith id
                match x.Actors.TryAdd(id.ToString(), actor) with
                | true -> actor
                | false -> x.Actors.GetOrAdd(id.ToString(), actor) :?> 'TActor

        member x.Store<'TActor when 'TActor : not struct> (actor: 'TActor) =
            let key = x.KeyAccessor.GetIdAsString actor
            x.Actors.TryAdd(key, actor) |> ignore