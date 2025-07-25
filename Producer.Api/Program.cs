using Azure.Messaging.ServiceBus;
using Producer.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Leer la configuración de ServiceBus
var serviceBusSection = builder.Configuration.GetSection("ServiceBus");
var connectionString = serviceBusSection["ConnectionString"];

// Registrar el ServiceBusClient con la connection string
builder.Services.AddSingleton(new ServiceBusClient(connectionString));

// Registrar tu servicio
builder.Services.AddSingleton<ServiceBusSenderService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
