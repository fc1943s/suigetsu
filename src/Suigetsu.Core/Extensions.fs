namespace Suigetsu.Core

open FSharpPlus
open FSharpPlus.Data


module Set =
    let toggle item set =
        if Set.contains item set
        then Set.add item set
        else Set.remove item set
        
    
module Async =
    let wrap x = async {
        return x
    } 

    let choose fn =
        Async.map (fn id)
    
    
module Result =
    let fold fn state =
        Seq.fold (fun state next ->
            match state, next with
            | Ok xs, Ok x -> fn xs x |> Ok
            | Error e, _
            | _, Error e -> e |> Error
        ) state
        
    let okOrThrow result =
        result
        |> Result.mapError (fun x -> x.ToString () |> exn)
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

