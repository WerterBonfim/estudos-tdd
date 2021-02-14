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

            var item = new PedidoItem(command.ProdutoId, command.Nome, command.Quantidade, command.ValorUnitario);
            var pedido = new Pedido(command.ClienteId);
            pedido.AdicionarItem(item);
            
            _pedidoRepository.Adicionar(pedido);
            

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