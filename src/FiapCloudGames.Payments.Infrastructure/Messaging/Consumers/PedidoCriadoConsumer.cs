using FiapCloudGames.Payments.Application.Services;
using FiapCloudGames.Payments.Domain.Pedidos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FiapCloudGames.Payments.Infrastructure.Messaging.Consumers
{
    public class PedidoCriadoConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConnectionFactory _factory;
        public PedidoCriadoConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;

            _factory = new ConnectionFactory
            {
                HostName = configuration.GetSection("RabbitMq:Host").Value!,
                UserName = configuration.GetSection("RabbitMq:Username").Value!,
                Password = configuration.GetSection("RabbitMq:Password").Value!,
                Port = Int32.Parse(configuration.GetSection("RabbitMq:Port").Value!)
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = await _factory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "pedido-criado",
                durable: true,
                exclusive: false,
                autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                var body = ea.Body.ToArray();

                var json = Encoding.UTF8.GetString(body);

                var evento = JsonSerializer.Deserialize<PedidoCriadoEvent>(json);

                if (evento is null)
                    return;

                using var scope = _serviceProvider.CreateScope();

                var handler = scope.ServiceProvider.GetRequiredService<ProcessarPagamentoService>();

                await handler.Processar(evento);
            };

            await channel.BasicConsumeAsync(queue: "pedido-criado", autoAck: true, consumer: consumer);
        }
    }
}
