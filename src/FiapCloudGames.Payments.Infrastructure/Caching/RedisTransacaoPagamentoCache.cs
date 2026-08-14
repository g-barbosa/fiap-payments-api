using FiapCloudGames.Payments.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace FiapCloudGames.Payments.Infrastructure.Caching
{
    /// <summary>
    /// Implementação de cache de transações de pagamento usando Redis.
    /// Armazena status e dados de pagamentos com TTL configurável.
    /// </summary>
    public class RedisTransacaoPagamentoCache : ITransacaoPagamentoCache
    {
        private readonly IDistributedCache _cache;
        private const string STATUS_KEY_PREFIX = "pagamento:status:";
        private const string TRANSACAO_KEY_PREFIX = "pagamento:transacao:";

        public RedisTransacaoPagamentoCache(IDistributedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<string?> ObterStatusAsync(Guid pedidoId)
        {
            try
            {
                var key = $"{STATUS_KEY_PREFIX}{pedidoId}";
                return await _cache.GetStringAsync(key);
            }
            catch
            {
                return null;
            }
        }

        public async Task AdicionarStatusAsync(Guid pedidoId, string status, int ttlSeconds = 3600)
        {
            try
            {
                var key = $"{STATUS_KEY_PREFIX}{pedidoId}";
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlSeconds)
                };

                await _cache.SetStringAsync(key, status, options);
            }
            catch
            {
                // Falhas no cache não devem impedir operação da aplicação
            }
        }

        public async Task RemoverAsync(Guid pedidoId)
        {
            try
            {
                var statusKey = $"{STATUS_KEY_PREFIX}{pedidoId}";
                var transacaoKey = $"{TRANSACAO_KEY_PREFIX}{pedidoId}";
                
                await _cache.RemoveAsync(statusKey);
                await _cache.RemoveAsync(transacaoKey);
            }
            catch
            {
                // Falhas no cache não devem impedir operação da aplicação
            }
        }

        public async Task AdicionarTransacaoAsync(Guid pedidoId, string dadosTransacao, int ttlSeconds = 3600)
        {
            try
            {
                var key = $"{TRANSACAO_KEY_PREFIX}{pedidoId}";
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlSeconds)
                };

                await _cache.SetStringAsync(key, dadosTransacao, options);
            }
            catch
            {
                // Falhas no cache não devem impedir operação da aplicação
            }
        }

        public async Task<string?> ObterTransacaoAsync(Guid pedidoId)
        {
            try
            {
                var key = $"{TRANSACAO_KEY_PREFIX}{pedidoId}";
                return await _cache.GetStringAsync(key);
            }
            catch
            {
                return null;
            }
        }
    }
}
