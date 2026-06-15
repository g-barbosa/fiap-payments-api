using FiapCloudGames.Payments.Domain.Events;
using FiapCloudGames.Payments.Domain.Interfaces.Messaging;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FiapCloudGames.Payments.Infrastructure.Messaging.Publishers
{
    public class RabbitMqPagamentoPublisher : IPagamentoProcessadoPublisher
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqPagamentoPublisher(IConfiguration configuration)
        {
            _factory = new ConnectionFactory
            {
                HostName = configuration.GetSection("RabbitMq:Host").Value!,
                UserName = configuration.GetSection("RabbitMq:Username").Value!,
                Password = configuration.GetSection("RabbitMq:Password").Value!,
                Port = Int32.Parse(configuration.GetSection("RabbitMq:Port").Value!)
            };
        }

        public async Task PublicarPagamentoProcessadoAsync(PagamentoProcessadoEvent evento)
        {
            var connection = await _factory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "pagamento-processado",
                durable: true,
                exclusive: false,
                autoDelete: false);

            var payload = JsonSerializer.Serialize(evento);

            var body = Encoding.UTF8.GetBytes(payload);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "pagamento-processado",
                mandatory: false,
                basicProperties: properties,
                body: body);
        }
    }
}
