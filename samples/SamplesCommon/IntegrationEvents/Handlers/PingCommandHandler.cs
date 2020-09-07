using SamplesCommon.IntegrationEvents.Events;
using System;
using System.Threading.Tasks;
using ZC.MQBus.Base.Handlers;

namespace SamplesCommon.IntegrationEvents.Handlers
{
    public class PingCommandHandler : ICommandHandler<PingCommand>
    {
        public Task Handle(PingCommand request)
        {
            Task.Delay(1000);

            Console.WriteLine($"{nameof(PingCommandHandler)} - {nameof(Handle)} - {request.Ping}");

            return Task.CompletedTask;
        }
    }
}
