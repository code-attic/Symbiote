#load "Module1.fs"
open Symbiote.Hollywood
open System.Collections.Concurrent
let count = 5000000
let actorLimit = 10000
let seq = seq { for i in 1 .. count -> (i%(count/(count/actorLimit)), i) };;

let state = new ConcurrentDictionary<string, int>()

let actor (id: string) (message: string) =
    state.AddOrUpdate(id, 1, new System.Func<string,int,int>(fun x y -> y+1)) |> ignore

let akshun = new System.Action<string, string>(actor)

let cast = new Agency<string>(akshun)

let postAsync (tuple: int * int) =
    async {
        let (id, i) = tuple
        let actorName = id.ToString()
        let msg = System.String.Format("Message {0}", i)
        cast.SendTo actorName msg
    }

let post (tuple: int * int) =
    let (id, i) = tuple
    let actorName = id.ToString()
    let msg = System.String.Format("Message {0}", i)
    printfn "%s" msg
    cast.SendTo actorName msg


let gogo() =
    let watch = System.Diagnostics.Stopwatch.StartNew()
    seq
    |> Seq.map postAsync
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore
    watch.Stop()

    printfn "There were %d actors provisioned to process the load in %d miliseconds" cast.Actors watch.ElapsedMilliseconds

gogo();;