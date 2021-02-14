using System;
using System.Threading.Tasks;
using NerdStore.Core.Data;
using NerdStore.Core.DomainObjects;

namespace NerdStore.Venda.Domain
{
    public interface IPedidoRepository : IRepository<Pedido>
    {
        void Adicionar(Pedido pedido);
        Task<Pedido> ObterPedidoRascunho(Guid clienteId);
        Task AdicionarItem(PedidoItem item);
        Task Atualizar(Pedido pedido);
    }
}