using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NerdStore.Venda.Application.Events;
using NerdStore.Venda.Domain;

namespace NerdStore.Venda.Application.Commands
{
    public class PedidoCommandHandler : IRequestHandler<AdicionarItemPedidoCommand, bool>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IMediator _mediator;


        public PedidoCommandHandler(IPedidoRepository pedidoRepository, IMediator mediator)
        {
            _pedidoRepository = pedidoRepository;
            _mediator = mediator;
        }


        public async Task<bool> Handle(AdicionarItemPedidoCommand command, CancellationToken cancellationToken)
        {
            if (!command.EstaValido())
                return false;

            var pedido = await _pedidoRepository.ObterPedidoRascunho(command.ClienteId);
            var item = new PedidoItem(command.ProdutoId, command.Nome, command.Quantidade, command.ValorUnitario);

            if (pedido == null)
            {
                pedido = new Pedido(command.ClienteId);
                pedido.AdicionarItem(item);

                _pedidoRepository.Adicionar(pedido);
            }
            else
            {
                var itemJaExisteNoPedido = pedido.Contem(item);
                pedido.AdicionarItem(item);

                if (itemJaExisteNoPedido) _pedidoRepository.AtualizarItem(item);
                else _pedidoRepository.AdicionarItem(item);

                _pedidoRepository.Atualizar(pedido);
            }

            var evento = new PedidoItemAdicionadoEvent(
                pedido.ClienteId,
                pedido.Id,
                item.ProdutoId,
                item.Titulo,
                item.ValorUnitario,
                item.Quantidade);

            //await _mediator.Publish(evento, cancellationToken);

            pedido.AdicionarEvento(evento);

            return await _pedidoRepository.UnitOfWork.Commit();
        }
    }
}