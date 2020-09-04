# ZC.MQBus

A library for the event-based, command-based and RCP-based communication using RabbitMQ.

 [![NuGet Badge](https://buildstats.info/nuget/ZC.MQBus.Base?includePreReleases=false)](https://www.nuget.org/packages/ZC.MQBus.Base)
 [![NuGet Badge](https://buildstats.info/nuget/ZC.MQBus.Rabbit?includePreReleases=false)](https://www.nuget.org/packages/ZC.MQBus.Rabbit)
 
## Samples

- [Publisher app](https://github.com/lzabik/ZC.EventBus/tree/master/samples/PublisherRPC)
- [Subscriber app](https://github.com/lzabik/ZC.EventBus/tree/master/samples/SubscriberRPC)
- [Framework Subscriber app](https://github.com/lzabik/ZC.EventBus/tree/master/samples/SampleFrameworkSubscriber)

## How-To

Install **`NuGet`** packages.

```console
PM> Install-Package ZC.MQBus.Base
```

Add configuration to **`appsettings.json`**.

```json
{
  "RabbitMq": {
    "BrokerName": "test_broker",        
    "RetryCount": "5",
    "VirtualHost": "your_virtual_host",
    "Username": "your_username",
    "Password": "your_password",
    "Host": "your_host"    
  }
}
```

If you want to use event, in **`publisher`** and **`subscriber`** apps, create a new class called
**`PingEvent`** (send to any subscriber).

```csharp
    public class PingEvent : IEvent
    {
        public string Ping { get; set; }
    }
```

If you want to use command, in **`publisher`** and **`subscriber`** apps, create a new class called
**`PingCommand`** (send to exactly one subscriber).

```csharp
    public class PingCommand: ICommand
    {
        public String Ping { get; set; }
    }
```

If you want to use RPC, in **`publisher`** and **`subscriber`** apps, create a new class called
**`RPCPingRequest`** (RPC is executed by exactly one subscriber and returns result).

```csharp
    public class RPCPingRequest : IRPCRequest<RPCPingResponse>
    {
        public String Ping { get; set; }
    }
```

In the **`subscriber`** app, create a new class called **`PingEventHandler`** in order to handle event.

```csharp
    public class PingEventHandler : IEventHandler<PingEvent>
    {
        public PingEventHandler()
        {
        }

        public async Task Handle(PingEvent @event)
        {
            Console.WriteLine($"{nameof(PingEventHandler)} - {nameof(Handle)} - {@event.Ping}");

            await Task.Yield();
        }
    }
```

In the **`subscriber`** app, create a new class called **`PingCommandHandler`** in order to handle command.

```csharp
    public class PingCommandHandler : ICommandHandler<PingCommand>
    {
        public Task Handle(PingCommand request)
        {
            Task.Delay(1000);

            Console.WriteLine($"{nameof(PingCommandHandler)} - {nameof(Handle)} - {request.Ping}");

            return Task.CompletedTask;
        }
    }
```

In the **`subscriber`** app, create a new class called **`RPCPingRequestHandler`** in order to handle RPC.

```csharp
    public class RPCPingRequestHandler : IRPCHandler<RPCPingRequest, RPCPingResponse>
    {
        public async Task<RPCPingResponse> Handle(RPCPingRequest request)
        {
            await Task.Delay(1000);

            Console.WriteLine($"{nameof(RPCPingRequestHandler)} - {nameof(Handle)} - {request.Ping}");

            return new RPCPingResponse() { Ping = "RPC Pong" };
        }
    }
```

In the **`subscriber`** app, modify **`ConfigureServices`** and **`Configure`** methods in **`Startup.cs`**.

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...

        var rabbitMqOptions = Configuration.GetSection("RabbitMq");
        services.Configure<RabbitMqOptions>(rabbitMqOptions);

        services.AddLogging();
        
        services.AddMessageBus();

        // test handlers
        services.AddTransient<PingEventHandler>();
        services.AddTransient<RPCPingRequestHandler>();
        services.AddTransient<PingCommandHandler>();        

        ...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ...
                
        var messageBus = ServiceProvider.GetRequiredService<IMessageBus>();

        messageBus.Subscribe<PingEvent, PingEventHandler>();
        messageBus.SubscribeRPC<RPCPingRequest, RPCPingRequestHandler, RPCPingResponse>();
        messageBus.SubscribeCommand<PingCommand, PingCommandHandler>();

        ...
    }
}
```

Publish the PingEvent event, PingCommand command or RPCPingRequest call in the **`publisher`** app by using the following code, for example in a controller.

```csharp
public class MessageBusController : ControllerBase
{
    private readonly IMessageBus _messageBus;

        public MessageBusController(IMessageBus eventBus)
        {
            _messageBus = eventBus;
        }

        [HttpPost("PushEvent")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult Publish()
        {
            var message = new PingEvent() { Ping = "Ping" };

            _messageBus.Publish(message);

            return Ok("A message has been published.");
        }

        [HttpPost("CallRPC")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CallRPC()
        {
            var result = await _messageBus.CallRPC<RPCPingRequest, RPCPingResponse>(new RPCPingRequest()
            {
                Ping = "Ping"
            });

            return Ok(result.Ping);
        }

        [HttpPost("PushCommand")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult PushCommand()
        {
            _messageBus.PushCommand(new PingCommand() { Ping = "Ping" });

            return Ok();
        }
    }
}
```

## References

- [RabbitMQ](https://www.rabbitmq.com/)
- [eShopOnContainers](https://github.com/dotnet-architecture/eShopOnContainers)
