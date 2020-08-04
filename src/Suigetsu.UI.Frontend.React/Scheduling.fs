namespace Suigetsu.UI.Frontend.React

open System
open Fable.Core
open Feliz


module Scheduling =
    type SchedulingType =
        | Timeout
        | Interval

    let private schedulingFn = function
        | Timeout -> JS.setTimeout, JS.clearTimeout
        | Interval -> JS.setInterval, JS.clearInterval

    let useScheduling schedulingType (fn: unit -> unit) duration =
        let savedCallback = React.useRef fn

        React.useEffect (fun () ->
            savedCallback.current <- fn
        , [| fn :> obj |])

        React.useEffect (fun () ->
            let set, clear = schedulingFn schedulingType
            let onExecute () =
                savedCallback.current ()

            let id = set onExecute duration

            { new IDisposable with
                member _.Dispose () =
                    clear id }
        , [| duration :> obj |])
