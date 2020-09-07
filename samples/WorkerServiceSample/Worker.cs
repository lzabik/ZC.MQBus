using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SamplesCommon.IntegrationEvents.Events;
using SamplesCommon.IntegrationEvents.Handlers;
using ZC.MQBus.Base;

namespace WorkerServiceSample
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageBus _eventBus = null;

        public Worker(ILogger<Worker> logger,
                      IMessageBus eventBus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _eventBus.Subscribe<PingEvent, PingEventHandler>();
            _eventBus.SubscribeRPC<RPCPingRequest, RPCPingRequestHandler, RPCPingResponse>();

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _eventBus.UnsubscribeEvent<PingEvent, PingEventHandler>();
            _eventBus.UnsubscribeRPC<RPCPingRequest, RPCPingRequestHandler, RPCPingResponse>();

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
