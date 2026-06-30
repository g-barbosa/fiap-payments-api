
using FiapCloudGames.Payments.Application.Services;
using FiapCloudGames.Payments.Domain.Interfaces.Messaging;
using FiapCloudGames.Payments.Infrastructure.Messaging.Consumers;
using FiapCloudGames.Payments.Infrastructure.Messaging.Publishers;

namespace FiapCloudGames.Payments.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            builder.Services.AddHealthChecks();

            builder.Services.AddScoped<IPagamentoProcessadoPublisher, RabbitMqPagamentoPublisher>();
            builder.Services.AddScoped<ProcessarPagamentoService>();
            builder.Services.AddHostedService<PedidoCriadoConsumer>();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}
