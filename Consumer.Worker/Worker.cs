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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = new ServiceBusClient(_config["ServiceBus:ConnectionString"]);
            _processor = client.CreateProcessor(
                _config["ServiceBus:TopicName"],
                _config["ServiceBus:SubscriptionName"]);

            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            await _processor.StopProcessingAsync(stoppingToken);
            await _processor.DisposeAsync();
        }
    }
}
