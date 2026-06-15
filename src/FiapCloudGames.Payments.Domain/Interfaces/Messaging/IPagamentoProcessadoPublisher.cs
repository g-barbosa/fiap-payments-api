using FiapCloudGames.Payments.Domain.Events;

namespace FiapCloudGames.Payments.Domain.Interfaces.Messaging
{
    public interface IPagamentoProcessadoPublisher
    {
        Task PublicarPagamentoProcessadoAsync(PagamentoProcessadoEvent evento);
    }
}
