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
open System.Collections.Concurrent

type Actor<'a> = MailboxProcessor<'a>

type Director<'TMessage>(call: Action<string, 'TMessage>) =
    
    let cast = new ConcurrentDictionary<string, Actor<'TMessage>>()

    let spawnActor id (call: Action<string, 'TMessage>) =
        let actor = 
            Actor<'TMessage>.Start ( fun channel ->
                let rec loop() =
                    async {
                        let! message = channel.Receive()
                        call.Invoke(id, message)
                        return! loop()
                    }
                loop() )
        actor
    
    let setupActor (call: Action<string, 'TMessage>) (id: string) (message: 'TMessage) =
        let actor = spawnActor id call
        actor.Post message
        cast.TryAdd(id, actor)
        

    new() = 
        let call id message = 
                    let line = message.ToString()
                    printfn "Actor %s says: '%s'" id line
        let act = new Action<string, 'TMessage>(call)
        Director(act)

    member x.call = call
    member x.Actors with get () = cast.Count

    member x.SendTo (id: string) (message: 'TMessage) = 
        let actor = cast.TryGetValue id
        match actor with
        | (true, channel) -> channel.Post message
        | (false, _) -> setupActor x.call id message |> ignore