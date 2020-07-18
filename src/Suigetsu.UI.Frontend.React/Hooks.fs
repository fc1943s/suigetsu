namespace Suigetsu.UI.Frontend.React

open System
open Fable.Core
open Fable.React

module Hooks =
    module Scheduling =
        type SchedulingType =
            | Timeout
            | Interval

        let schedulingFn = function
            | Timeout -> JS.setTimeout, JS.clearTimeout
            | Interval -> JS.setInterval, JS.clearInterval

    let useScheduling schedulingType fn interval =
        let savedCallback = Hooks.useRef fn

        Hooks.useEffect (fun () ->
            savedCallback.current <- fn
        , [| fn |])

        Hooks.useEffectDisposable (fun () ->
            let set, clear = Scheduling.schedulingFn schedulingType
            let onExecute () =
                savedCallback.current ()

            let id = set onExecute interval

            { new IDisposable with
                member _.Dispose () =
                    clear id }
        , [| interval |])
