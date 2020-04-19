namespace Suigetsu.Core

module Tuple2 =
    let map fn (a, b) =
        fn a, fn b
  
    let mapFst fn (a, b) =
        fn a, b
        
    let mapSnd fn (a, b) =
        a, fn b

