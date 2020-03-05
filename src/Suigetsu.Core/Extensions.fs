namespace Suigetsu.Core

module Set =
    let toggle item set =
        if Set.contains item set
        then Set.add item set
        else Set.remove item set
        
module Map =
   let keys map =
       map
       |> Map.toSeq
       |> Seq.map fst
       
   let values map =
       map
       |> Map.toSeq
       |> Seq.map snd
    
module Async =
    let map fn afn = async {
        let! x = afn
        let value = fn x
        return value
    }
        
    let choose fn =
        map (fn id)
    
module Result =
    let mapFn succ err = function
        | Ok x -> succ x
        | Error ex -> err ex
        
    let value = function
       | Ok x -> x
       | Error ex -> exn (ex.ToString ()) |> raise
       
    let tryFn msg fn =
        try
            fn () |> Ok
        with ex ->
            msg ex |> Error

