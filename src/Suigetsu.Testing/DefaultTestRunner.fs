namespace Suigetsu.Testing

open Xunit.Abstractions
open Suigetsu.CoreCLR
open Serilog

type DefaultTestRunner (output: ITestOutputHelper) =
    do Logging.addLoggingSink (fun x -> x.WriteTo.TestOutput output) false
