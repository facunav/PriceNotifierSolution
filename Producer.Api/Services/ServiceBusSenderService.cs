using Azure.Messaging.ServiceBus;
using Shared.Models;
using System.Text.Json;

namespace Producer.Api.Services
{
    public class ServiceBusSenderService
    {
        private readonly ServiceBusClient _client;
        private readonly string _topicName;

        public ServiceBusSenderService(ServiceBusClient client, IConfiguration config)
        {
            _client = client;
            _topicName = config["ServiceBus:TopicName"];
        }

        public async Task SendMessageAsync(PriceUpdateMessage message)
        {
            var sender = _client.CreateSender(_topicName);

            var sbMessage = new ServiceBusMessage(JsonSerializer.Serialize(message));
            sbMessage.ApplicationProperties.Add("tipo", message.Tipo);

            await sender.SendMessageAsync(sbMessage);
        }
    }
}
