using FiapCloudGames.Payments.Application.Interfaces;
using FiapCloudGames.Payments.Domain.Events;
using FiapCloudGames.Payments.Domain.Interfaces.Messaging;
using FiapCloudGames.Payments.Domain.Pedidos;
using System.Text.Json;

namespace FiapCloudGames.Payments.Application.Services
{
    /// <summary>
    /// Serviço para processar pagamentos com cache de transações.
    /// </summary>
    public class ProcessarPagamentoService
    {
        private readonly IPagamentoProcessadoPublisher _pagamentoProcessadoPublisher;
        private readonly ITransacaoPagamentoCache _transacaoCache;

        public ProcessarPagamentoService(
            IPagamentoProcessadoPublisher pagamentoProcessadoPublisher,
            ITransacaoPagamentoCache transacaoCache)
        {
            _pagamentoProcessadoPublisher = pagamentoProcessadoPublisher;
            _transacaoCache = transacaoCache;
        }

        public async Task Processar(PedidoCriadoEvent evento)
        {
            var status = Random.Shared.Next(2) == 0 ? "Aprovado" : "Recusado";

            var pagamentoProcessado = new PagamentoProcessadoEvent
            {
                PedidoId = evento.PedidoId,
                NomeUsuario = evento.NomeUsuario,
                Email = evento.Email,
                Valor = evento.Valor,
                Status = status,
                IdJogo = evento.IdJogo,
                IdBiblioteca = evento.IdBiblioteca
            };

            // Cachear status e dados da transação
            await _transacaoCache.AdicionarStatusAsync(evento.PedidoId, status);
            
            var dadosTransacao = JsonSerializer.Serialize(pagamentoProcessado);
            await _transacaoCache.AdicionarTransacaoAsync(evento.PedidoId, dadosTransacao);

            await _pagamentoProcessadoPublisher.PublicarPagamentoProcessadoAsync(pagamentoProcessado);
        }
    }
}
