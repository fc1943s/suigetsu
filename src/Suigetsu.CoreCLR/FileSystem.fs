namespace Suigetsu.CoreCLR

open Microsoft.Extensions.FileProviders
open System
open Serilog
open System.IO
open System.Reflection
open System.Threading.Tasks
    
module FileSystem =

    type private FileSystemChange =
        | Changed of FileSystemEventArgs
        | Created of FileSystemEventArgs
        | Deleted of FileSystemEventArgs
        | Renamed of RenamedEventArgs

    let rec private getStreamAsync path = async {
        try
            return new FileStream (path, FileMode.Open, FileAccess.Write)
        with _ -> 
            Log.Warning ("Error opening file for writing. Waiting... Path: {Path}", path)
            do! Task.Delay (TimeSpan.FromSeconds 1.) |> Async.AwaitTask
            return! getStreamAsync path
    }

    let createTempFile path ext =
        let rawTempPath = Path.GetTempFileName ()
        
        let newPath = rawTempPath.Replace (".tmp", sprintf ".%s" ext)
        let newPath = Path.Combine (path, Path.GetFileName newPath)
        
        File.Move (rawTempPath, newPath)
        newPath
        
    let ensureTempSessionDirectory () =
        let tempFolder = Path.Combine (Path.GetTempPath (), Assembly.GetEntryAssembly().GetName().Name, string (Guid.NewGuid ()))
        Directory.CreateDirectory tempFolder |> ignore
        
        tempFolder
                                        
    let private waitForWritingAsync path = async {
       do! getStreamAsync path |> Async.Ignore
    }

    let private watchFileSystem2 path handler =
        Log.Debug ("watchFileSystem Start")
        
        use fileProvider = new PhysicalFileProvider (path)
        let rec watch () =
            let notify (state: obj) =
                Log.Verbose ("File change detected: {State}", state)
                
                handler ()
                watch ()
                
            let token = fileProvider.Watch "*.*"
            token.RegisterChangeCallback ((fun x -> notify x), null) |> ignore
        
        watch ()
        Console.ReadKey false |> ignore
        
        
    let private watchFileSystem path handler =
        Log.Debug ("watchFileSystem Start")
        
        use watcher = new FileSystemWatcher (Path = path, EnableRaisingEvents = true)
        
        let callback (change: FileSystemChange) =
            Log.Verbose ("File change detected: {Change}", change)
            handler change
        
        watcher.Changed.Add (fun args -> FileSystemChange.Changed args |> callback)
        watcher.Created.Add (fun args -> FileSystemChange.Created args |> callback)
        watcher.Deleted.Add (fun args -> FileSystemChange.Deleted args |> callback)
        watcher.Renamed.Add (fun args -> FileSystemChange.Renamed args |> callback)
        
        Console.ReadKey false |> ignore

