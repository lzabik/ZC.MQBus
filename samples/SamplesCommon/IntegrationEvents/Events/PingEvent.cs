using ZC.MQBus.Base.Messages;

namespace SamplesCommon.IntegrationEvents.Events
{
    public class PingEvent : IEvent
    {
        public string Ping { get; set; }
    }
}
