using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using NerdStore.Venda.Application.Commands;
using NerdStore.Venda.Domain;
using Xunit;

namespace NerdStore.Venda.Application.Testes.Pedidos
{
    public class PedidoCommandHandlerTestes
    {
        [Fact(DisplayName = "Deve adicionar um novo item no pedido com sucesso")]
        [Trait("Handler", "Adicionar")]
        public async Task  DeveAdicionarUmNovoItemNoPedidoComSucesso()
        {
            // Arrange
            var command = new AdicionarItemPedidoCommand(Guid.NewGuid(), Guid.NewGuid(), 
                "produto teste", 2, 100);

            var mocker = new AutoMocker();
            var pedidoHandler = mocker.CreateInstance<PedidoCommandHandler>();

            mocker.GetMock<IPedidoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .Returns(Task.FromResult(true));
            
            // Act

            var resultado = await pedidoHandler.Handle(command, CancellationToken.None);

            // Assert

            resultado.Should().BeTrue();
            
            mocker.GetMock<IPedidoRepository>()
                .Verify( x => x.Adicionar(It.IsAny<Pedido>()), Times.Once);
            
            mocker.GetMock<IPedidoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            
            // mocker.GetMock<IMediator>()
            //     .Verify(x => 
            //         x.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
        }
    }
}