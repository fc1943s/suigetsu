namespace Suigetsu.Core

open System
    
module Runtime =
    let getStackTraceIo () =
        Environment.StackTrace.Split (Environment.NewLine.ToCharArray (), StringSplitOptions.RemoveEmptyEntries)
        
