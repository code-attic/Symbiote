open System.Collections.Concurrent

type Actor<'a> = MailboxProcessor<'a>

type Cast<'T>() =

    let cast = new ConcurrentDictionary<string, Actor<'T>>()
    let spawnActor id =
        printfn "Creating new actor, '%s'" id
        let actor = 
            Actor<'T>.Start ( fun channel ->
                let rec loop() =
                    async {
                        let! message = channel.TryReceive()
                        match message with
                        | Some(message) -> printfn "Actor %s says: '%s'" id (message.ToString())
                        | _ -> printfn "Timeout. oh crap"
                        return! loop()
                    }
                loop() )
        actor

    let CreateActorAndSendTo (id: string) (message: 'T) =
        let actor = spawnActor id
        actor.Post message
        cast.TryAdd(id, actor)

    member x.Actors with get () = cast

    member x.Say (id: string) (message: 'T) = 
        //let actor = x.Actors.TryGetValue id
        let actor = cast.TryGetValue id
        match actor with
        | (true, channel) -> channel.Post message
        | _ -> CreateActorAndSendTo id message |> printfn "%b"

        printfn "There are now %d actors" cast.Count



let achTeam = new Cast<string>()
achTeam.Say "alex" "Man, I'm tired"
achTeam.Say "jim" "Why, were you riding my mom *all* night long?"
achTeam.Say "everyone" "/cry"
achTeam.Say "alex" "Why, Jim, WHY!?"
achTeam.Say "jim" "Because, Alex, because I can. And I must. I can and I must and I can't help it."
