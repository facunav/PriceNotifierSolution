using Azure.Messaging.ServiceBus;
using Shared.Models;
using System.Text.Json;

namespace Consumer.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IConfiguration _config;
        private ServiceBusProcessor _processor;
        private readonly ILogger<Worker> _logger;

        public Worker(IConfiguration config, ILogger<Worker> logger)
        {
            _config = config;
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var client = new ServiceBusClient(_config["ServiceBus:ConnectionString"]);
            _processor = client.CreateProcessor(
                _config["ServiceBus:TopicName"],
                _config["ServiceBus:SubscriptionName"]);

            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(cancellationToken);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var json = args.Message.Body.ToString();
            var message = JsonSerializer.Deserialize<PriceUpdateMessage>(json);

            _logger.LogInformation("[GENERAL] Mensaje recibido: {Contenido}", message?.Contenido);

            await args.CompleteMessageAsync(args.Message);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
    }
}
