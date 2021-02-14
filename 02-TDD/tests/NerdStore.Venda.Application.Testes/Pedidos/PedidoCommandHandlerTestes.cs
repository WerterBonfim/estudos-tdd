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
        [Fact(DisplayName = "Deve adicionar um item para um novo pedido")]
        [Trait("Handler", "Adicionar Novo Pedido")]
        public async Task DeveAdicionarUmNovoItemParaUmPedidoNovoComSucesso()
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
                .Verify(x => x.Adicionar(It.IsAny<Pedido>()), Times.Once);

            mocker.GetMock<IPedidoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);

            // mocker.GetMock<IMediator>()
            //     .Verify(x => 
            //         x.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
        }

        [Fact(DisplayName = "Deve adicionar um novo item no pedido com sucesso")]
        [Trait("Handler", "Adicionar Novo Item")]
        public async Task DeveAdicionarUmNovoItemNoPedidoComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();

            var pedido = new Pedido(clienteId);
            var item = new PedidoItem(Guid.NewGuid(), "Produto teste", 2, 100);
            pedido.AdicionarItem(item);

            var command = new AdicionarItemPedidoCommand(
                clienteId,
                Guid.NewGuid(),
                "produto novo",
                1,
                100);

            var mocker = new AutoMocker();
            var pedidoHandler = mocker.CreateInstance<PedidoCommandHandler>();

            var repository = mocker.GetMock<IPedidoRepository>();

            repository
                .Setup(x => x.ObterPedidoRascunho(clienteId))
                .Returns(Task.FromResult(pedido));

            repository
                .Setup(x => x.UnitOfWork.Commit())
                .Returns(Task.FromResult(true));

            // Act

            var resultado = await pedidoHandler.Handle(command, CancellationToken.None);

            // Assert

            resultado.Should().BeTrue();

            repository
                .Verify(x =>
                    x.AdicionarItem(It.IsAny<PedidoItem>()), Times.Once);

            repository
                .Verify(x =>
                    x.Atualizar(It.IsAny<Pedido>()), Times.Once);

            repository
                .Verify(x =>
                    x.UnitOfWork.Commit(), Times.Once);
        }

        [Fact(DisplayName = "Deve atualizar um item previamente adicionado")]
        [Trait("Handler", "Atualizar Item")]
        public async Task DeveAtualizarItemPreviamenteAdicionado()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var produtoId = Guid.NewGuid();

            var pedido = new Pedido(clienteId);
            var item = new PedidoItem(produtoId, "Produto teste", 2, 100);
            pedido.AdicionarItem(item);

            var command = new AdicionarItemPedidoCommand(
                clienteId,
                produtoId,
                "Produto teste",
                2,
                100);

            var mocker = new AutoMocker();
            var pedidoHandler = mocker.CreateInstance<PedidoCommandHandler>();

            var repository = mocker.GetMock<IPedidoRepository>();

            repository
                .Setup(x => x.ObterPedidoRascunho(clienteId))
                .Returns(Task.FromResult(pedido));

            repository
                .Setup(x => x.UnitOfWork.Commit())
                .Returns(Task.FromResult(true));


            // Act

            var resultado = await pedidoHandler.Handle(command, CancellationToken.None);

            // Assert

            resultado.Should().BeTrue();

            repository
                .Verify(x =>
                    x.AtualizarItem(It.IsAny<PedidoItem>()), Times.Once);

            repository
                .Verify(x =>
                    x.Atualizar(It.IsAny<Pedido>()), Times.Once);

            repository
                .Verify(x =>
                    x.UnitOfWork.Commit(), Times.Once);
        }

        [Fact(DisplayName = "Deve notificar erros para um command inválido")]
        [Trait("Handler", "Validacao")]
        public async Task DeveNotificarErrosParaUmCommandInvalido()
        {
            // Arrange
            var command = new AdicionarItemPedidoCommand(
                Guid.Empty, Guid.Empty, "", 0, 0);


            var mocker = new AutoMocker();
            var pedidoHandler = mocker.CreateInstance<PedidoCommandHandler>();

            // Act

            var resultado = await pedidoHandler.Handle(command, CancellationToken.None);

            // Assert

            resultado.Should().BeFalse();

            mocker.GetMock<IMediator>()
                .Verify(x =>
                    x.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Exactly(5));
            
            
        }
    }
}