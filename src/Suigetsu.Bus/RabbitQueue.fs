namespace Suigetsu.Bus

open RabbitMQ.Client
open System
open EasyNetQ
open Serilog

module RabbitQueue =

    type private NamingConventions =
        { RetrieveQueueName: Type -> string -> string
          RetrieveExchangeName: Type -> string
          RetrieveErrorQueueName: MessageReceivedInfo -> string
          RetrieveErrorExchangeName: MessageReceivedInfo -> string }
        static member inline Default =
            { RetrieveQueueName = fun messageType subscriptionId -> "@@Queue: " + (messageType.FullName + subscriptionId)
              RetrieveExchangeName = fun messageType -> "@@Exchange: " + messageType.FullName
              RetrieveErrorQueueName = fun messageReceivedInfo -> messageReceivedInfo.Queue + " - @@Error"
              RetrieveErrorExchangeName = fun messageReceivedInfo -> messageReceivedInfo.Exchange + " - @@Error" }
        
    type private BusConnection =
        { Address: string
          Port: uint16
          Username: string
          Password: string }
        
    let private withNamingConventions (namingConventions: NamingConventions) (bus: IBus) =
        bus.Advanced.Conventions.QueueNamingConvention <- QueueNameConvention namingConventions.RetrieveQueueName
        bus.Advanced.Conventions.ExchangeNamingConvention <- ExchangeNameConvention namingConventions.RetrieveExchangeName
        bus.Advanced.Conventions.ErrorQueueNamingConvention <- ErrorQueueNameConvention namingConventions.RetrieveErrorQueueName
        bus.Advanced.Conventions.ErrorExchangeNamingConvention <- ErrorExchangeNameConvention namingConventions.RetrieveErrorExchangeName
        bus
        
    let private buildConnectionString busConnection =
        sprintf "host=%s:%d;username=%s;password=%s"
            busConnection.Address busConnection.Port busConnection.Username busConnection.Password
            
    let createBus address username password =
        { Address = address
          Port = uint16 5672
          Username = username
          Password = password }
        |> buildConnectionString
        |> RabbitHutch.CreateBus
        |> withNamingConventions NamingConventions.Default
        
        

    type Exchange<'T when 'T: not struct> (bus: IBus) =
        
        let exchange = bus.Advanced.ExchangeDeclare (sprintf "@@Exchange-%s" typeof<'T>.FullName, ExchangeType.Topic)
        
        member _.Post routingKey message =
            try
                bus.Advanced.Publish<'T> (exchange, routingKey, false, Message message)
            with ex ->
                Log.Error (ex, "Error while publishing message: {A}", message)
            
            
        member this.RegisterConsumer bindingKeys (handler:'T -> Exchange<'T> -> Async<unit>) =
            let queue = bus.Advanced.QueueDeclare ()
            
            for bindingKey in bindingKeys do
                bus.Advanced.Bind (exchange, queue, bindingKey) |> ignore
            
            async {
                let onMessage (body: IMessage<'T>) (__info: MessageReceivedInfo) =
                    handler body.Body this |> Async.Start
                    
                use _ = bus.Advanced.Consume (queue, onMessage)
                
                while true do
                    do! Async.Sleep 1000
            } |> Async.Start
            
