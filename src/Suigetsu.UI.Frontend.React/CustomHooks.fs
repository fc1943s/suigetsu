namespace Suigetsu.UI.Frontend.React

open System
open Browser
open Browser.Types
open Feliz


module CustomHooks =
    let useWindowSize () =
        let getWindowSize () =
            {| Width = window.innerWidth
               Height = window.innerHeight |}
        let size, setSize = React.useState (getWindowSize ())

        React.useLayoutEffect (fun () ->
            let updateSize (_event: Event) =
                setSize (getWindowSize ())

            window.addEventListener ("resize", updateSize)

            { new IDisposable with
                member _.Dispose () =
                    window.removeEventListener ("resize", updateSize)
            }
        )
        size

