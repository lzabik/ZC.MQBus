using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamplesCommon.IntegrationEvents.Events;
using SamplesCommon.IntegrationEvents.Handlers;
using ZC.MQBus.Base;

namespace Publisher.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
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
            var connectionString = await _messageBus.CallRPC<RPCPingRequest, RPCPingResponse>(new RPCPingRequest()
            {
                Ping = "Ping"
            });

            return Ok(connectionString.Ping);
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
