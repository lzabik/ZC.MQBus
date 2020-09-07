using SamplesCommon.IntegrationEvents.Events;
using System;
using System.Threading.Tasks;
using ZC.MQBus.Base.Bus;
using ZC.MQBus.Base.Handlers;

namespace SamplesCommon.IntegrationEvents.Handlers
{
    public class RPCPingResponse
    {
        public String Ping { get; set; }
    }
    public class RPCPingRequestHandler : IRPCHandler<RPCPingRequest, RPCPingResponse>
    {
        public async Task<RPCPingResponse> Handle(RPCPingRequest request)
        {
            await Task.Delay(1000);

            Console.WriteLine($"{nameof(RPCPingRequestHandler)} - {nameof(Handle)} - {request.Ping}");

            return new RPCPingResponse() { Ping = "RPC Pong" };
        }
    }
}
