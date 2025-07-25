using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Producer.Api.Services;
using Shared.Models;
using System.Text.Json;

namespace Producer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ServiceBusSenderService _sender;

        public MessageController(ServiceBusSenderService sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] PriceUpdateMessage message)
        {
            await _sender.SendMessageAsync(message);
            return Ok("Mensaje enviado");
        }
    }
}
