namespace FiapCloudGames.Payments.Application.Interfaces
{
    /// <summary>
    /// Interface para cache de transações de pagamento.
    /// Armazena status e resultados de pagamentos temporariamente.
    /// </summary>
    public interface ITransacaoPagamentoCache
    {
        /// <summary>
        /// Obter status de transação do cache por ID do pedido.
        /// </summary>
        /// <param name="pedidoId">ID do pedido</param>
        /// <returns>Status da transação ou null se não encontrado</returns>
        Task<string?> ObterStatusAsync(Guid pedidoId);

        /// <summary>
        /// Armazenar status de transação no cache.
        /// </summary>
        /// <param name="pedidoId">ID do pedido</param>
        /// <param name="status">Status da transação (Aprovado, Recusado, etc)</param>
        /// <param name="ttlSeconds">Tempo de expiração em segundos (padrão: 3600s = 1h)</param>
        Task AdicionarStatusAsync(Guid pedidoId, string status, int ttlSeconds = 3600);

        /// <summary>
        /// Remover transação do cache.
        /// </summary>
        /// <param name="pedidoId">ID do pedido</param>
        Task RemoverAsync(Guid pedidoId);

        /// <summary>
        /// Armazenar dados completos da transação.
        /// </summary>
        /// <param name="pedidoId">ID do pedido</param>
        /// <param name="dadosTransacao">JSON serializado dos dados da transação</param>
        /// <param name="ttlSeconds">Tempo de expiração em segundos</param>
        Task AdicionarTransacaoAsync(Guid pedidoId, string dadosTransacao, int ttlSeconds = 3600);

        /// <summary>
        /// Obter dados completos da transação.
        /// </summary>
        /// <param name="pedidoId">ID do pedido</param>
        /// <returns>JSON serializado ou null</returns>
        Task<string?> ObterTransacaoAsync(Guid pedidoId);
    }
}
