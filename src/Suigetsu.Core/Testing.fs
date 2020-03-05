namespace Suigetsu.Core

open System.Reflection

module Testing =
    let isTestingLazyIo =
        fun () -> Assembly.GetEntryAssembly().GetName().Name = "testhost"
        |> Core.memoizeLazy
