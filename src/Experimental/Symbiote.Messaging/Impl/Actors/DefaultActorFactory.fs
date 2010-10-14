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

open Symbiote.Messaging.Impl
open Microsoft.Practices.ServiceLocation

type DefaultActorFactory<'TActor when 'TActor : not struct>
    (keyAccessor: IKeyAccessor) =

    member x.KeyAccessor
        with get() = keyAccessor

    interface IActorFactory<'TActor> with
        
        member x.CreateInstance<'TKey> (id: 'TKey) =
            let actor = ServiceLocator.Current.GetInstance<'TActor>()
            x.KeyAccessor.SetId actor id
            actor