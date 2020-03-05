namespace Suigetsu.Core

open System

module CoreConfig =
    let getRequiredEnvVarIo name =
        match Environment.GetEnvironmentVariable name with
        | null | "" -> sprintf "Invalid EnvVar: %s" name |> Error
        | s -> s |> Ok

