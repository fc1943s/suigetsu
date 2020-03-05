namespace Suigetsu.Core

open System
open Serilog

module Queue =

    type IQueue<'T> =
        inherit IDisposable
        
        abstract Post: 'T -> unit

    type Queue<'T> (handler:'T -> IQueue<'T> -> Async<unit>) as this =
        
        let queue = MailboxProcessor<'T>.Start (fun inbox -> async {
            Log.Information ("Starting queue")
            
            while true do
                let! message = inbox.Receive ()

                Log.Verbose (">")
                Log.Verbose ("Queue message received: {Message}. Current Queue Length: {Length}", message.GetType().Name, inbox.CurrentQueueLength)
                
                async {
                    try
                        do! handler message this
                    with ex ->
                        Log.Error (ex, "Error dispatching queue message.")
                } |> Async.Start
        })
        
        interface IQueue<'T> with
            override this.Post message = queue.Post message
            
        interface IDisposable with
            override _.Dispose () =
                ()
        
