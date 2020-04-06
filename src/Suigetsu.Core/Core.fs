namespace Suigetsu.Core

open System.Collections.Generic

module Core =
    let rec recFn fn x =
        fn (recFn fn) x
    
    let memoize fn =
        let cache = Dictionary<_,_> ()
        fun k ->
            match cache.TryGetValue k with
            | true, v -> v
            | false, _ ->
                let v = fn k
                cache.Add (k,v)
                v

    let memoizeAsync fn = 
        let cache = Dictionary<_,_> ()
        fun k -> async {
            match cache.TryGetValue k with
            | true, v -> v
            | false, _ ->
                let! v = fn k
                cache.Add (k,v)
                v
        }
        
    let memoizeLazy fn =
        let result = lazy (fn ())
        fun () -> result.Value
