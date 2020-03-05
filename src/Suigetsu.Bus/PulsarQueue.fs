namespace Suigetsu.Bus

open Pulsar.Client.Api
open Serilog
open Suigetsu.Core

open System
open System.Collections.Concurrent
open FSharp.Control.Tasks.V2
open MBrace.FsPickler
open Pulsar.Client.Common
open Serilog.Extensions.Logging

module PulsarQueue =
        
    let private pickleRegistry = CustomPicklerRegistry ()
    //pickleRegistry.DeclareSerializable<Stopwatch> ()
    //pickleRegistry.DeclareSerializable<LocalQueue.Sample> ()
    //pickleRegistry.DeclareSerializable<QueryResult> ()
    //pickleRegistry.DeclareSerializable<QueryStats> ()
    //pickleRegistry.DeclareSerializable<ResultEntry> ()
    //pickleRegistry.DeclareSerializable<ExtendedResultEntry> ()
    //pickleRegistry.DeclareSerializable<Coverage> ()
    //pickleRegistry.DeclareSerializable<MatchedWith> ()

    #nowarn "8989"
    let private pickleCache = PicklerCache.FromCustomPicklerRegistry pickleRegistry
        
    let mutable private _logSet = false

        
    type PulsarQueue<'T> () =
        
        do
            if not _logSet then
                PulsarClient.Logger <- (new SerilogLoggerProvider (Log.Logger)).CreateLogger typeof<PulsarClient>.Name
                _logSet <- true
        
        let serviceUrl = "pulsar://wsl.local:6650"
        let topicName = sprintf "%s" typeof<'T>.Name
        
        
        let binarySerializer = FsPickler.CreateBinarySerializer (picklerResolver = pickleCache)
        let client = PulsarClientBuilder().ServiceUrl(serviceUrl).Build ()
        
        do (async {
            Log.Information ("Starting queue")
            
            // let reader =
            //     ReaderBuilder(client)
            //         .Topic(topicName)
            //         .ReaderName(string (Random().Next()))
            //         .StartMessageId(MessageId.Latest)
            //         .CreateAsync() |> Async.AwaitTask |> Async.RunSynchronously
                
            let! consumer =
                ConsumerBuilder(client)
                    .Topic(topicName)
                    .SubscriptionName(string (Random().Next()))
                    .ConsumerName(string (Random().Next()))
                    .SubscriptionType(SubscriptionType.Shared)
                    .AckTimeout(TimeSpan.FromSeconds(20.))
                    .SubscribeAsync() |> Async.AwaitTask
                    
            do!
                task {
                    while true do
                        try
                            let! rawMessage = consumer.ReceiveAsync()
                        //  let! rawMessage = reader.ReadNextAsync()

                            let message = binarySerializer.UnPickle<'T> rawMessage.Data
                            message |> ignore
                            
                            do! consumer.AcknowledgeAsync(rawMessage.MessageId)
                        with ex ->
                            Log.Error(ex, "Error consuming message")
                } |> Async.AwaitTask
            
        } |> Async.Start)

        let createProducer () =
            ProducerBuilder(client)
                .Topic(topicName)
                .ProducerName(string (Random().Next()))
                .EnableBatching(false)
            //  .SendTimeout(TimeSpan.FromSeconds(4.))
                .CreateAsync() |> Async.AwaitTask |> Async.RunSynchronously
            
        let producerPool = ConcurrentBag<_> ()
            
        let fetchProducer callback =
            Log.Verbose("Fetching producer. Pool length: {Len}", producerPool.Count)
                
            seq { 1..3 }
            |> Seq.tryFind (fun _ ->
                    try
                        if producerPool.IsEmpty then
                            producerPool.Add(createProducer())
                            
                        let producer = producerPool.TryTake () |> snd
                        
                        callback producer
                        
                        producerPool.Add producer
                        true
                    with ex ->
                        Log.Error(ex, "Error adding message on pulsar queue"); false )
            |> ignore
            
        
        interface Queue.IQueue<'T> with
            override _.Post message =
                fetchProducer (fun producer ->
                    let pickle = binarySerializer.Pickle message
                    let messageId = producer.SendAsync(pickle).GetAwaiter().GetResult()
                    Log.Verbose("Message produced. Id: {Id}", messageId))

        interface IDisposable with
            override _.Dispose () =
                ()
