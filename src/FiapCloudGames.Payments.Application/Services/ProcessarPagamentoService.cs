using FiapCloudGames.Payments.Domain.Events;
using FiapCloudGames.Payments.Domain.Interfaces.Messaging;
using FiapCloudGames.Payments.Domain.Pedidos;

namespace FiapCloudGames.Payments.Application.Services
{
    public class ProcessarPagamentoService
    {
        private readonly IPagamentoProcessadoPublisher _pagamentoProcessadoPublisher;
        public ProcessarPagamentoService(IPagamentoProcessadoPublisher pagamentoProcessadoPublisher)
        {
            _pagamentoProcessadoPublisher = pagamentoProcessadoPublisher;
        }
        public async Task Processar(PedidoCriadoEvent evento)
        {
            var status = Random.Shared.Next(2) == 0 ? "Aprovado" : "Recusado";

            await _pagamentoProcessadoPublisher.PublicarPagamentoProcessadoAsync(new PagamentoProcessadoEvent
            {
                PedidoId = evento.PedidoId,
                NomeUsuario = evento.NomeUsuario,
                Email = evento.Email,
                Valor = evento.Valor,
                Status = status,
                IdJogo = evento.IdJogo,
                IdBiblioteca = evento.IdBiblioteca
            });
        }
    }
}
