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

open System.Collections.Concurrent

type Actor<'a> = MailboxProcessor<'a>

type Director<'T>(callback: string -> 'T -> unit) =
    
    let cast = new ConcurrentDictionary<string, Actor<'T>>()
    let spawnActor (callback: string -> 'T -> unit) id =
        let actor = 
            Actor<'T>.Start ( fun channel ->
                let rec loop() =
                    async {
                        let! message = channel.Receive()
                        callback id message
                        return! loop()
                    }
                loop() )
        actor
    
    let setupActor (callback: (string -> 'T -> unit)) (id: string) (message: 'T) =
        let actor = spawnActor callback id
        actor.Post message
        cast.TryAdd(id, actor)

    new() = 
        let callback id message = 
                    let line = message.ToString()
                    printfn "Actor %s says: '%s'" id line
        Director(callback)

    member x.Callback = callback
    member x.Actors with get () = cast.Count

    member x.SendTo (id: string) (message: 'T) = 
        let actor = cast.TryGetValue id
        match actor with
        | (true, channel) -> channel.Post message
        | (false, _) -> setupActor x.Callback id message |> ignore