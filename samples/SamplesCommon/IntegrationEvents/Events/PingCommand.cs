using System;
using System.Collections.Generic;
using System.Text;
using ZC.MQBus.Base.Messages;

namespace SamplesCommon.IntegrationEvents.Events
{
    public class PingCommand: ICommand
    {
        public String Ping { get; set; }
    }
}
