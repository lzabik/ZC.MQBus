using SamplesCommon.IntegrationEvents.Events;
using System;
using System.Threading.Tasks;
using ZC.MQBus.Base.Handlers;

namespace SamplesCommon.IntegrationEvents.Handlers
{
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
}
