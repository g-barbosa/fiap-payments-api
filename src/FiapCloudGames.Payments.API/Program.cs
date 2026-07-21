
using FiapCloudGames.Payments.API.Middleware;
using FiapCloudGames.Payments.Application.Services;
using FiapCloudGames.Payments.Domain.Interfaces.Messaging;
using FiapCloudGames.Payments.Infrastructure.Messaging.Consumers;
using FiapCloudGames.Payments.Infrastructure.Messaging.Publishers;
using Serilog;

namespace FiapCloudGames.Payments.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            builder.Host.UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext();
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "FIAP Cloud Games API - API de Pagamentos"
                });
            });

            var rabbitMqHost = builder.Configuration["RabbitMq:Host"] ?? "rabbitmq";
            var rabbitMqPort = int.Parse(builder.Configuration["RabbitMq:Port"] ?? "5672");
            var rabbitMqUri = new Uri($"amqp://admin:rabbitmq123@{rabbitMqHost}:{rabbitMqPort}/");

            builder.Services.AddHealthChecks();

            builder.Services.AddScoped<IPagamentoProcessadoPublisher, RabbitMqPagamentoPublisher>();
            builder.Services.AddScoped<ProcessarPagamentoService>();
            builder.Services.AddHostedService<PedidoCriadoConsumer>();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}
