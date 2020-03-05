namespace Suigetsu.Core

[<AutoOpen>]
module Operators =
    (* Exclusive 'between' operator *)
    let inline ( >< ) x (min, max) =
        (x > min) && (x < max)

    (* Inclusive 'between' operator *)
    let inline ( >=< ) x (min, max) =
        (x >= min) && (x <= max)

    let inline flip fn y x =
        fn x y

