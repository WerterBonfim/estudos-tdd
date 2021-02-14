using NerdStore.Core.Data;
using NerdStore.Core.DomainObjects;

namespace NerdStore.Venda.Domain
{
    public interface IPedidoRepository : IRepository<Pedido>
    {
        void Adicionar(Pedido pedido);
    }
}