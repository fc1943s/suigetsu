namespace Suigetsu.CoreCLR

open Serilog
open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks
open System.Linq
open System.Collections.Generic

module Async =
    
    // TODO: Is Async.Sequential bugged? :|
    let sequentialForced<'T> =
        Seq.map (fun x -> async { return x |> Async.RunSynchronously } )
        >> Async.Sequential<'T>
    
    let throttle n fns = seq {
        let n = new Threading.Semaphore (n, n)
        for fn in fns -> async {
            do! Async.AwaitWaitHandle n |> Async.Ignore
            let! result = Async.Catch fn
            n.Release () |> ignore
            return result |> function
                | Choice1Of2 result -> result
                | Choice2Of2 ex -> raise ex
        }
    }

    let handleParallel<'T> =
        Seq.map (fun fn -> async {
            try
                do! fn 
            with ex ->
                Log.Error (ex, "Error running parallel task")
        })

[<AutoOpen>]
module Extensions =

    type IEnumerable<'T> with
        member this.FirstOrDefault (predicate, defaultValue) =
            let value =
                match predicate with
                | Some predicate -> this.FirstOrDefault predicate
                | None -> this.FirstOrDefault ()
                        
            if Object.Equals (value, Unchecked.defaultof<'T>)
            then defaultValue ()
            else value
            
        member this.FirstOrDefault defaultValue =
            this.FirstOrDefault (None, defaultValue)


    type Process with
        member this.WaitForExitAsync (?cancellationToken) = async {
            let cancellationToken = defaultArg cancellationToken Unchecked.defaultof<CancellationToken>
            
            let completion = TaskCompletionSource<bool> ()
            
            let onProcessExit = EventHandler (fun obj args ->
                async { completion.TrySetResult true |> ignore } |> Async.Start
            )
                
            this.EnableRaisingEvents <- true
            this.Exited.AddHandler onProcessExit
            
            try
                if not this.HasExited then
                    cancellationToken.Register (fun _ -> async { completion.TrySetCanceled () |> ignore } |> Async.Start) |> ignore
                    do! completion.Task |> Async.AwaitTask |> Async.Ignore
            finally
                this.Exited.RemoveHandler onProcessExit
        }
        
        
