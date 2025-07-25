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

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var client = new ServiceBusClient(_config["ServiceBus:ConnectionString"]);
            _processor = client.CreateProcessor(
                _config["ServiceBus:TopicName"],
                _config["ServiceBus:FacturasSubscriptionName"]);

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

            _logger.LogInformation("⚠️ FACTURA recibida: {Contenido}", message?.Contenido);

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
