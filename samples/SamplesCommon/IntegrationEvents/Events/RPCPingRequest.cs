using SamplesCommon.IntegrationEvents.Handlers;
using System;
using ZC.MQBus.Base.Bus;
using ZC.MQBus.Base.Handlers;

namespace SamplesCommon.IntegrationEvents.Events
{
    public class RPCPingRequest : IRPCRequest<RPCPingResponse>
    {
        public String Ping { get; set; }
    }
}
