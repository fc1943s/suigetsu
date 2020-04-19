namespace Suigetsu.Core

open FSharpPlus
open FSharpPlus.Data


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
    
    let wrap x = async {
        return x
    } 

    let choose fn =
        map (fn id)
    
module Result =
    let fold fn state =
        Seq.fold (fun state next ->
            match state, next with
            | Ok ys, Ok y -> fn ys y |> Ok
            | Error e, _ -> Error e
            | _, Error e -> Error e
        ) state
        
    let mapFn succ err = function
        | Ok x -> succ x
        | Error ex -> err ex
        
    let value = function
       | Ok x -> x
       | Error ex -> exn (ex.ToString ()) |> raise
       
    let okOrThrow result =
        result
        |> Result.mapError exn
        |> ResultOrException.Result
        
    let collect items =
        items
        |> Seq.toList
        |> Result.partition
        |> Tuple2.mapFst (List.collect id)
       
    let tryFn msg fn =
        try
            fn () |> Ok
        with ex ->
            msg ex |> Error

