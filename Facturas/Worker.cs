using Azure.Messaging.ServiceBus;
using Shared.Models;
using System.Text.Json;

namespace Facturas
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private ServiceBusProcessor _processor;
        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
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

            _logger.LogInformation("⚠️ FACTURA recibida: {Contenido}", message?.Contenido);

            await args.CompleteMessageAsync(args.Message);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = new ServiceBusClient(_config["ServiceBus:ConnectionString"]);
            _processor = client.CreateProcessor(
                _config["ServiceBus:TopicName"],
                _config["ServiceBus:FacturasSubscriptionName"]);

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
