using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NerdStore.Core.DomainObjects;
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
            {
                await NotificarErros(command);
                return false;
            }

            var pedido = await _pedidoRepository.ObterPedidoRascunho(command.ClienteId);
            var item = new PedidoItem(command.ProdutoId, command.Nome, command.Quantidade, command.ValorUnitario);

            if (pedido == null)
                pedido = CriarNovoPedido(command, item);
            else
                AtualizarItem(pedido, item);

            return await EfetuarCommit(pedido, item);
        }

        private async Task NotificarErros(AdicionarItemPedidoCommand command)
        {
            foreach (var error in command.Validacoes.Errors)
            {
                var notification = new DomainNotification(command.MessageType, error.ErrorMessage);
                await _mediator.Publish(notification, CancellationToken.None);
            }
        }

        private Pedido CriarNovoPedido(AdicionarItemPedidoCommand command, PedidoItem item)
        {
            var pedido = new Pedido(command.ClienteId);
            pedido.AdicionarItem(item);

            _pedidoRepository.Adicionar(pedido);
            return pedido;
        }

        private void AtualizarItem(Pedido pedido, PedidoItem item)
        {
            var itemJaExisteNoPedido = pedido.Contem(item);
            pedido.AdicionarItem(item);

            if (itemJaExisteNoPedido) _pedidoRepository.AtualizarItem(item);
            else _pedidoRepository.AdicionarItem(item);

            _pedidoRepository.Atualizar(pedido);
        }

        private async Task<bool> EfetuarCommit(Pedido pedido, PedidoItem item)
        {
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