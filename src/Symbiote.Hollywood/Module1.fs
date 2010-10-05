// Learn more about F# at http://fsharp.net

module Symbiote.Hollywood

open System.Collections.Concurrent

type Actor<'a> = MailboxProcessor<'a>

type Agency<'T>(callback: System.Action<string, 'T>) =
    
    let cast = new ConcurrentDictionary<string, Actor<'T>>()
    let spawnActor (callback: System.Action<string, 'T>) id =
        let actor = 
            Actor<'T>.Start ( fun channel ->
                let rec loop() =
                    async {
                        let! message = channel.Receive()
                        callback.Invoke(id, message)
                        return! loop()
                    }
                loop() )
        actor
    
    let setupActor (callback: System.Action<string, 'T>) (id: string) (message: 'T) =
        let actor = spawnActor callback id
        actor.Post message
        cast.TryAdd(id, actor)

    new() = 
        let callback id message = 
                    let line = message.ToString()
                    printfn "Actor %s says: '%s'" id line
        let action = new System.Action<string, 'T>(callback)
        Agency(action)

    member x.Callback = callback
    member x.Actors with get () = cast.Count

    member x.SendTo (id: string) (message: 'T) = 
        let actor = cast.TryGetValue id
        match actor with
        | (true, channel) -> channel.Post message
        | (false, _) -> setupActor x.Callback id message |> ignore
